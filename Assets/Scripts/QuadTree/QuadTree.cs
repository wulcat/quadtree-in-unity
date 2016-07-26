using UnityEngine;
using System.Collections.Generic ;
// using System.Collections ;
// [System.SerializableAttribute]
public class QuadTree {










	// private Rect size ;
	private int maxTargets ; //
	private int maxLevels ;
	private int levels ;
	// private List<Rect> objects ;
	
	public List<QuadTree> Nodes = new List<QuadTree>() ; //
	private object sortLock = new object() ;
	public Bounds mapSize ; //
	public List<BoxCollider> targets = new List<BoxCollider>() ; //
	// public List<Collider> targetCollider ; //
	

	public QuadTree(Rect _size , int _maxTargets , int _maxLevels , int _levels) {
		// size = _size ;
		maxTargets = _maxTargets ;
		maxLevels = _maxLevels ;
		levels = _levels ;

		mapSize.center = new Vector3(_size.x,_size.y,0); //
		var min = new Vector3(_size.x - _size.width/2 ,_size.y - _size.height/2) ; //
		var max = new Vector3(_size.y + _size.width/2 ,_size.y + _size.height/2) ; //
		mapSize.SetMinMax(min , max);
	}
	// public void Move(Vector2 position) {
	// 	mapSize.center = position ;
	// 	// mapSize.min = new Vector3(position.x - mapSize.min.x ,position.y - mapSize.min.y) ;
	// 	// mapSize.max = new Vector3(position.x + mapSize.max.x ,position.y + mapSize.max.y) ; 
	// 	// mapSize.min = 
	// }
	public void split() {
		lock (sortLock)
		{
			var new_level = maxLevels + 1;
			// var width = (mapSize.max.x - mapSize.center.x) ; 
			var side_width = mapSize.extents.x ;
			var side_height = mapSize.extents.y ;
			// Debug.Log(side_width +","+ mapSize.max.x +","+ mapSize.center.x+","+ mapSize.size.x);
			// var side_depth = mapSize.max.z - mapSize.center.z ;
			// var x = mapSize.center.x ;
			// var y = mapSize.center.y ;

			if(Nodes.Count == 0 || Nodes.Count < 4) {
				Nodes = new List<QuadTree>(new QuadTree[4]) ;
			}

			Nodes[0] = new QuadTree(
				new Rect(mapSize.max.x , mapSize.min.y , side_width , side_height) ,
				maxTargets ,
				maxLevels ,
				new_level
			) ;

			Nodes[1] = new QuadTree(
				new Rect(mapSize.min.x , mapSize.min.y , side_width , side_height) ,
				maxTargets ,
				maxLevels ,
				new_level
			) ;
			Nodes[2] = new QuadTree(
				new Rect(mapSize.min.x , mapSize.max.y , side_width , side_height) ,
				maxTargets ,
				maxLevels ,
				new_level
			) ;
			Nodes[3] = new QuadTree(
				new Rect(mapSize.max.x , mapSize.max.y , side_width , side_height) ,
				maxTargets ,
				maxLevels ,
				new_level
			) ;
		}
		
	}
	public int getId(Bounds bound) {
		lock (sortLock)
		{
			var rectId = -1 ;
			var x_midpoint = mapSize.center.x ;
			var y_midpoint = mapSize.center.y ;

			var top_quad = (bound.min.y < y_midpoint && bound.max.y < y_midpoint) ;
			var bottom_quad = bound.min.y > y_midpoint ;

			if(bound.min.x < x_midpoint && bound.max.x < x_midpoint) {
				if(top_quad) rectId = 1;
				else if (bottom_quad) rectId = 2;
			}
			else if (bound.min.x > x_midpoint) {
				if(top_quad) rectId = 0 ;
				else if(bottom_quad) rectId = 3;
			}
			return rectId ;
		}
		
	}
	public void insert(BoxCollider collider) {
		lock(sortLock) {
			var i = 0 ;
			var rank = 0 ;


			if(Nodes.Count > 0 && Nodes[0] != null) {
				rank = getId(collider.bounds) ;
				if(rank != -1) {
					Nodes[rank].insert(collider) ;
					return ;
				}
			}
			targets.Add(collider) ;

			if(targets.Count > maxTargets && levels < maxLevels) {
				if(Nodes[0] == null) split() ;
				while (i < targets.Count) {
					rank = getId(targets[i].bounds) ;
					if(rank != -1) {
						var t = targets[i] ;
						targets.Remove(targets[i]) ;
						// Debug.Log(t);
						Nodes[rank].insert(t) ;
					}
					else {
						i++ ;
					}
				}
			}
		}
	}
	public BoxCollider[] retrieve(BoxCollider collider) {
		lock (sortLock)
		{
			var rank = getId(collider.bounds) ;
			var getTargets = targets ;
			// Debug.Log
			// Debug.Log("Targets : "+getTargets.Count);
			// Debug.Log(rank);
			// if(Nodes.Count == 0) split() ;


			// Debug.Log(Nodes.Count);
			// if(Nodes) Debug
			// Debug.Log(Nodes.Count);
			// Debug.Log(Nodes[0].GetType().ToString());// + Nodes[0].mapSize.center);
			// Debug.Log(Nodes[0].mapSize.center);
			if(Nodes.Count != 0) {
				if(rank != -1) {
					// Debug.Log(rank);
					var re = Nodes[rank].retrieve(collider);
					// getTargets.AddRange(re);
					if(re.Length != 0)
					getTargets.InsertRange(getTargets.Count -1, Nodes[rank].retrieve(collider) ) ;
				}
				else {
					for(var i = 0 ; i < Nodes.Count ; i++) {
						// Debug.Log(Nodes[i]);
						// Debug.Log(Nodes[i]+"i"+i);
						var re = Nodes[i].retrieve(collider) ;
						// getTargets.AddRange(re);
						if(re.Length != 0)
						getTargets.InsertRange(getTargets.Count -1 , re) ;
					}
				}
			}
			// Debug.Log("GetTargets : "+getTargets.Count);
			return getTargets.ToArray() ;
		}
		
	}
	public void clear() {
		lock (sortLock)
		{
			targets.Clear() ;
			for(var i = 0 ; i < Nodes.Count ; i++) {
				if(Nodes[i] != null) {
					Nodes[i].clear() ;
				}
			}
			Nodes.Clear() ;
		}
		
	}
	
}
