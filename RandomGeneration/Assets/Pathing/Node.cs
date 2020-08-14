using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

	public bool walkable;
	public Vector2 worldPos,gridPos;
	public int gCost,hCost;
	public Node parent;
	int heapIndex;


	public Node(bool _walkable,Vector2 _worldPos,Vector2 _gridPos){
		walkable = _walkable;
		worldPos = _worldPos;
		gridPos = _gridPos;
	}

	public int fCost{
		get{
			return gCost + hCost;
		}
	}

	public int HeapIndex{
		get{
			return heapIndex;
		}
		set{
			heapIndex = value;
		}
	}

	public int CompareTo(Node node){
		int compare = fCost.CompareTo(node.fCost);
		if(compare ==0){
			compare = hCost.CompareTo(node.hCost);
		}
		return -compare;
	}
}
