using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridHandler : MonoBehaviour {
	public bool displayGrid;
	public Transform player;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;
	float nodeDiameter;

    [SerializeField]
    Tilemap wall;



	void Awake () {
		player = GameObject.Find("objPlayer").transform;
		nodeDiameter = nodeRadius * 2;
	}

    private void Start() {
        CreateGrid();
    }

    public int MaxSize {
		get {
			return Mathf.RoundToInt (gridWorldSize.x * gridWorldSize.y);
		}
	}

	void Update(){	}

	public void CreateGrid () {
        grid = new Node[0, 0];
		grid = new Node[(int)gridWorldSize.x, (int)gridWorldSize.y];
        Vector2 worldBottomLeft = Vector2.zero;
		for (int x = 0; x < gridWorldSize.x; x++) {
			for (int y = 0; y < gridWorldSize.y; y++) {
                bool walkable = wall.GetTile(new Vector3Int(x, y, 0)) != null ? false : true;
                    
                    //!(Physics2D.OverlapCircle (worldPoint, nodeRadius - .05f, unwalkableMask));
				grid [x, y] = new Node (walkable, new Vector2(x,y), new Vector2 (x, y));
			}
		}

	}

	public List<Node> GetNeighbors (Node node) {
		List<Node> neighbors = new List<Node> ();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0||x==-1 && y==-1||x==1&&y==-1||x==-1&y==1||x==1&&y==1) {
					continue;
				}

				int checkX = (int)node.gridPos.x + x;
				int checkY = (int)node.gridPos.y + y;
				if (checkX >= 0 && checkX < gridWorldSize.x &&
				    checkY >= 0 && checkY < gridWorldSize.y) {
					neighbors.Add (grid [checkX, checkY]);
				}
			}
		}
		return neighbors;
	}

	public Node NodeFromWorldPos (Vector2 worldPosition) {
		return grid [Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y)];
	}


	void OnDrawGizmos () {
		if (displayGrid) {
			Gizmos.DrawWireCube (transform.position, gridWorldSize);
			if (grid != null) {
				Node playerNode = NodeFromWorldPos (player.position);
				foreach (Node n in grid) {
					Gizmos.color = (n.walkable) ? Color.white : Color.red;
					if (playerNode == n) {
						Gizmos.color = Color.blue;
					}
					Gizmos.DrawCube (n.worldPos, Vector3.one * (nodeDiameter - .1f));
				}
			}
		}
	}
}
