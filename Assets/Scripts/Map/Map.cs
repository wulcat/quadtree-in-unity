using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	// public Rect rect ;

	public BoxCollider main ;
	public QuadTree quadTree = new QuadTree(
		new Rect(0,0,1000,1000),
		4,
		4,
		4
	) ;
	private List<GameObject> tar = new List<GameObject>() ;
	public BoxCollider[] quadReturns ;
	// Use this for initialization
	void Start () {

		// quadTree.Move(transform.position);
		quadTree.clear() ;
		quadTree.split() ;
		tar.AddRange(GameObject.FindGameObjectsWithTag("quad-targets")) ;
		for(var i = 0 ; i < tar.Count ; i++) {
			quadTree.insert(tar[i].GetComponent<BoxCollider>()) ;
		}
		// transform.localScale = new Vector3(quadTree.mapSize.max.x , 0 , quadTree.mapSize.max.y);
	}
	
	// Update is called once per frame
	void Update () {
		quadReturns = null ;
		
		quadTree.split() ;
		tar.Clear() ;
		tar.AddRange(GameObject.FindGameObjectsWithTag("quad-targets")) ;
		
		for(var i = 0 ; i < tar.Count ; i++) {
			tar[i].GetComponent<MeshRenderer>().material.color = Color.blue ;
			quadTree.insert(tar[i].GetComponent<BoxCollider>()) ;
		}
		quadReturns = quadTree.retrieve(main);
		
		for(var i = 0 ; i < quadReturns.Length ; i++) {
			quadReturns[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.red ;
		}
		quadTree.clear() ;

	}
	void OnDrawGizmos() {
		var bounds = quadTree.mapSize ;
		Gizmos.color = Color.red ;
		// Gizmos.DrawWireCube(transform.position , new Vector3(bounds.size.x , bounds.size.y));
		// for(var i = 0 ; i < quadTree.Nodes.Count ; i++) {
		// 	bounds = quadTree.Nodes[i].mapSize ;
		// 	Gizmos.color = Color.green ;
		// 	Gizmos.DrawWireCube(new Vector3(bounds.center.x , bounds.center.y) , new Vector3(bounds.size.x , bounds.size.y));
		// }
		DrawLoop(quadTree);
	}
	void DrawLoop(QuadTree quad) {
		var bounds = quad.mapSize ;
		// for(var i = 0 ; i < quadTree.Nodes.Count ; i++) {
		// 	bounds = quadTree.Nodes[i].mapSize ;
		// 	Gizmos.color = Color.green ;
		Gizmos.DrawWireCube(new Vector3(bounds.center.x , bounds.center.y) , new Vector3(bounds.size.x , bounds.size.y));
		// }

		for(var i = 0 ; i < quad.Nodes.Count ; i++) {
			DrawLoop(quad.Nodes[i]);
		}
		
	}
}
