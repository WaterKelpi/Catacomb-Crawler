using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;
	GridHandler grid;

	void Awake () {
		requestManager = GetComponent<PathRequestManager> ();
		grid = GetComponent<GridHandler> ();
	}

	public void StartFindPath (Vector2 startPos, Vector2 endPos) {
        //StartCoroutine (FindPath (startPos, endPos));
        FindPath(startPos, endPos);
	}

	void FindPath (Vector2 startPos, Vector2 targetPos) {
		Vector2[] waypoints = new Vector2[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPos (startPos);
		Node targetNode = grid.NodeFromWorldPos (targetPos);
		if (startNode.walkable && targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node> ();
			openSet.Add (startNode);
			while (openSet.Count > 0) {
				Node curNode = openSet.RemoveFirst ();
				closedSet.Add (curNode);
				if (curNode == targetNode) {
					pathSuccess = true;
					break;
				}
				foreach (Node neighbor in grid.GetNeighbors(curNode)) {
					if (!neighbor.walkable || closedSet.Contains (neighbor)) {
						continue;
					}
					int newMoveCostToNeighbor = curNode.gCost + GetDistance (curNode, neighbor);
					if (newMoveCostToNeighbor < neighbor.gCost || !openSet.Contains (neighbor)) {
						neighbor.gCost = newMoveCostToNeighbor;
						neighbor.hCost = GetDistance (neighbor, targetNode);
						neighbor.parent = curNode;
						if (!openSet.Contains (neighbor)) {
							openSet.Add (neighbor);
						} else {
							openSet.UpdateItem (neighbor);
						}
					}
				}
			}
		}
		//yield return null;
		if (pathSuccess) {
			waypoints = RetracePath (startNode, targetNode);
		}
		requestManager.FinishedProcessingPath (waypoints, pathSuccess);
	}

	Vector2[] RetracePath (Node startNode, Node endNode) {
		List<Node> path = new List<Node> ();
		Node curNode = endNode;
		while (curNode != startNode) {
			path.Add (curNode);
			curNode = curNode.parent;
		}
		path.Add(startNode);
		Vector2[] waypoints = SimplifyPath (path);
		Array.Reverse (waypoints);
		return waypoints;

	}

	Vector2[] SimplifyPath (List<Node> path) {
		List<Vector2> waypoints = new List<Vector2> ();
		Vector2 directionOld = Vector2.zero;
		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2 (path [i - 1].gridPos.x - path [i].gridPos.x, path [i - 1].gridPos.x - path [i].gridPos.y);
			if (directionNew != directionOld) {
				waypoints.Add (path [i-1].worldPos);
			}
			directionOld = directionNew;
		}  
		return waypoints.ToArray ();
	}


	int GetDistance (Node nodeA, Node nodeB) {
		int distX = Mathf.Abs (Mathf.RoundToInt (nodeA.gridPos.x - nodeB.gridPos.x));
		int distY = Mathf.Abs (Mathf.RoundToInt (nodeA.gridPos.y - nodeB.gridPos.y));
		if (distX > distY) {
			return 14 * distY + 10 * (distX - distY);
		}
		return 14 * distX + 10 * (distY - distX);

	}


}
