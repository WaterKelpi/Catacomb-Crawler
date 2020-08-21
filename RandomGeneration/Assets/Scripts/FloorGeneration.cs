using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorGeneration : MonoBehaviour {
    [SerializeField]
    private Tilemap groundMap,wallMap,trapMap;
    [SerializeField]
    private RuleTile wall, floor,carpet;
    [SerializeField]
    GameObject itemPrefab;
    [SerializeField]
    Item defaultItem;


	public GameObject bgRoom, bgFloor, bgConnection;
	int cellWidth, cellHeight;
	public tileType[,] floorArray;

    public GameObject stairs;

	public class Corridor {
		public Vector2 position;

		public Corridor (Vector2 pos) {
			position = pos;
		}
	}

	// Use this for initialization
	void Start () {
		//floorGen (floorW, floorH, minRoomSize, maxRoomSize);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public tileType[,] floorGen (FloorInfo floorInfo) {
		floorArray = new tileType[floorInfo.floorWidth, floorInfo.floorHeight];
		//Sets entire array to void tiles
		for (int xx = 0; xx < floorInfo.floorWidth; xx++) {
			for (int yy = 0; yy < floorInfo.floorHeight; yy++) {
				floorArray [xx, yy] = tileType.VOID;
			}	
		}
		//Check the max number of rooms that can be spawned
		int maxNumRooms = (floorInfo.floorWidth * floorInfo.floorHeight) / (floorInfo.roomMax * floorInfo.roomMax);
        //Generate Item Room
        if (!floorInfo.noItemRoom) {
            while (true) {
                if (GenerateRoom(5, 5, tileType.ITEMROOM)) {
                    break;
                }
            }
        }
        //Generate Shop
        if (!floorInfo.noShop) {
            while (true) {
                if (GenerateRoom(5, 5, tileType.SHOPROOM)) {
                    break;
                }
            }
        }
        


        //Spawn the rooms
        for (int i = 0; i < Mathf.Clamp(maxNumRooms * Random.Range(.5f, 1.5f) - 2,2, maxNumRooms*Random.Range(.5f,1.5f)-2); i++) {

            int roomHeight = Random.Range(floorInfo.roomMin, floorInfo.roomMax);
            roomHeight += roomHeight % 2 == 0 ? 1 : 0;
            int roomWidth = Random.Range(floorInfo.roomMin, floorInfo.roomMax);
            roomWidth += roomWidth % 2 == 0 ? 1 : 0;
            int roomGenAttempts = 0;
            while (true) {
                roomGenAttempts++;
                if (GenerateRoom(roomWidth, roomHeight, tileType.ROOM)) {
                    break;
                }
                if (roomGenAttempts > 100) {
                    if (i < 2) {
                        Debug.Log("Cannot generate enough rooms. Either floor is too small, or rooms are too big.");
                    }
                    break;
                }
            }

		}
		//Generates the corridors
		for (int xx = 1; xx < floorInfo.floorWidth - 1; xx+=2) {
			for (int yy = 1; yy < floorInfo.floorHeight - 1; yy+=2) {
				GenerateCorridor (new Vector2 (xx, yy));
			}
		}
		//Creates possible connections
		for (int xx = 1; xx < floorInfo.floorWidth - 1; xx++) {
			for (int yy = 1; yy < floorInfo.floorHeight - 1; yy++) {
				if(floorArray[xx,yy] == tileType.VOID){
                    //Floor Left, Room Right
					if (floorArray [xx - 1, yy] == tileType.FLOOR && floorArray [xx + 1, yy] == tileType.ROOM || floorArray[xx - 1, yy] == tileType.FLOOR && floorArray[xx + 1, yy] == tileType.ITEMROOM || floorArray[xx - 1, yy] == tileType.FLOOR && floorArray[xx + 1, yy] == tileType.SHOPROOM) {
						floorArray [xx, yy] = tileType.CONNECTION;
					}
                    //Floor Right, Room Left
					if (floorArray [xx - 1, yy] == tileType.ROOM && floorArray [xx + 1, yy] == tileType.FLOOR || floorArray[xx - 1, yy] == tileType.ITEMROOM && floorArray[xx + 1, yy] == tileType.FLOOR || floorArray[xx - 1, yy] == tileType.SHOPROOM && floorArray[xx + 1, yy] == tileType.FLOOR) {
						floorArray [xx, yy] = tileType.CONNECTION;
					}
                    //Room Left, Room Right
					if (floorArray [xx - 1, yy] == tileType.ROOM && floorArray [xx + 1, yy] == tileType.ROOM) {
						floorArray [xx, yy] = tileType.CONNECTION;
					}
                    //Floor Down, Room Up
					if (floorArray [xx, yy - 1] == tileType.FLOOR && floorArray [xx, yy + 1] == tileType.ROOM|| floorArray[xx, yy - 1] == tileType.FLOOR && floorArray[xx, yy + 1] == tileType.ITEMROOM || floorArray[xx, yy - 1] == tileType.FLOOR && floorArray[xx, yy + 1] == tileType.SHOPROOM) {
						floorArray [xx, yy] = tileType.CONNECTION;
					}
                    //Floor Up, Room Down
					if (floorArray [xx, yy - 1] == tileType.ROOM && floorArray [xx, yy + 1] == tileType.FLOOR || floorArray[xx, yy - 1] == tileType.ITEMROOM && floorArray[xx, yy + 1] == tileType.FLOOR || floorArray[xx, yy - 1] == tileType.SHOPROOM && floorArray[xx, yy + 1] == tileType.FLOOR) {
						floorArray [xx, yy] = tileType.CONNECTION;
					}
                    //Room Up, Room Down
					if (floorArray [xx, yy - 1] == tileType.ROOM && floorArray [xx, yy + 1] == tileType.ROOM || floorArray[xx, yy - 1] == tileType.ROOM && floorArray[xx, yy + 1] == tileType.ITEMROOM|| floorArray[xx, yy - 1] == tileType.ROOM && floorArray[xx, yy + 1] == tileType.SHOPROOM) {
						floorArray [xx, yy] = tileType.CONNECTION;
					}
				}
			}
		}

        //Create connections for rooms
        for (int xx = 1; xx < floorInfo.floorWidth - 1; xx++) {
            for (int yy = 1; yy < floorInfo.floorHeight - 1; yy++) {
                //Found upper lefthand corner of room
                if (floorArray[xx, yy] == tileType.ROOM || floorArray[xx, yy] == tileType.ITEMROOM || floorArray[xx, yy] == tileType.SHOPROOM) {
                    int roomWidth = 0;
                    int roomHeight=0;
                    while (floorArray[xx + roomWidth, yy] == tileType.ROOM || floorArray[xx + roomWidth, yy] == tileType.ITEMROOM || floorArray[xx + roomWidth, yy] == tileType.SHOPROOM) {
                        roomWidth++;
                    }
                    while (floorArray[xx, yy+roomHeight] == tileType.ROOM || floorArray[xx, yy + roomHeight] == tileType.ITEMROOM || floorArray[xx, yy + roomHeight] == tileType.SHOPROOM) {
                        roomHeight++;
                    }
                    List<Vector2Int> northConnections = new List<Vector2Int>();
                    List<Vector2Int> southConnections = new List<Vector2Int>();
                    List<Vector2Int> eastConnections = new List<Vector2Int>();
                    List<Vector2Int> westConnections = new List<Vector2Int>();
                    for (int roomX = -1; roomX < roomWidth+1; roomX++) {
                        for (int roomY = -1; roomY < roomHeight+1; roomY++) {
                            if (floorArray[xx + roomX, yy + roomY] == tileType.CONNECTION) {
                                if (roomX == -1) {eastConnections.Add(new Vector2Int(xx + roomX, yy + roomY));}
                                if (roomX == roomWidth) { westConnections.Add(new Vector2Int(xx + roomX, yy + roomY)); }

                                if (roomY == -1) {northConnections.Add(new Vector2Int(xx + roomX, yy + roomY));}
                                if (roomY == roomHeight) {southConnections.Add(new Vector2Int(xx + roomX, yy + roomY));}
                            }
                        }
                    }
                    List<string> connectionDirs = new List<string>();
                    for (int i = 0; i < (roomWidth * roomHeight) / (floorInfo.roomMin * floorInfo.roomMin);i++) {
                        connectionDirs.Clear();
                        if (northConnections.Any()) { connectionDirs.Add("N"); }
                        if (eastConnections.Any()) { connectionDirs.Add("E"); }
                        if (southConnections.Any()) { connectionDirs.Add("S"); }
                        if (westConnections.Any()) { connectionDirs.Add("W"); }

                        Vector2Int newConnection;
                        if (connectionDirs.Count == 0) { continue; }
                        
                        switch (connectionDirs[Random.Range(0, connectionDirs.Count)]) {
                            case "N":
                                newConnection = northConnections[Random.Range(0, northConnections.Count)];
                                foreach (Vector2Int cellLocation in northConnections) { floorArray[cellLocation.x, cellLocation.y] = tileType.VOID; }
                                floorArray[newConnection.x, newConnection.y] = tileType.FLOOR;
                                northConnections.Clear();
                                break;
                            case "S":
                                newConnection = southConnections[Random.Range(0, southConnections.Count)];
                                foreach (Vector2Int cellLocation in southConnections) { floorArray[cellLocation.x, cellLocation.y] = tileType.VOID; }
                                floorArray[newConnection.x, newConnection.y] = tileType.FLOOR;
                                southConnections.Clear();
                                break;
                            case "E":
                                newConnection = eastConnections[Random.Range(0, eastConnections.Count)];
                                foreach (Vector2Int cellLocation in eastConnections) { floorArray[cellLocation.x, cellLocation.y] = tileType.VOID; }
                                floorArray[newConnection.x, newConnection.y] = tileType.FLOOR;
                                eastConnections.Clear();
                                break;
                            case "W":
                                newConnection = westConnections[Random.Range(0, westConnections.Count)];
                                foreach (Vector2Int cellLocation in westConnections) { floorArray[cellLocation.x, cellLocation.y] = tileType.VOID; }
                                floorArray[newConnection.x, newConnection.y] = tileType.FLOOR;
                                westConnections.Clear();
                                break;
                            default:
                                break;
                        }
                    }
                    foreach (Vector2Int cellLocation in northConnections) { floorArray[cellLocation.x, cellLocation.y] = tileType.VOID; }
                    foreach (Vector2Int cellLocation in southConnections) { floorArray[cellLocation.x, cellLocation.y] = tileType.VOID; }
                    foreach (Vector2Int cellLocation in eastConnections) { floorArray[cellLocation.x, cellLocation.y] = tileType.VOID; }
                    foreach (Vector2Int cellLocation in westConnections) { floorArray[cellLocation.x, cellLocation.y] = tileType.VOID; }
                }
            }
        }

		//Remove the dead ends

		for (int i = 0; i < 50; i++) {
			for (int xx = 1; xx < floorInfo.floorWidth - 1; xx++) {
				for (int yy = 1; yy < floorInfo.floorHeight - 1; yy++) {
					//If the block is an endcap to a hall way
					if (floorArray [xx - 1, yy] == tileType.VOID && floorArray [xx + 1, yy] == tileType.VOID && floorArray [xx, yy - 1] == tileType.VOID && floorArray [xx, yy + 1] == tileType.FLOOR) {
						floorArray [xx, yy] = tileType.VOID;
					}
					if (floorArray [xx - 1, yy] == tileType.VOID && floorArray [xx + 1, yy] == tileType.VOID && floorArray [xx, yy - 1] == tileType.FLOOR && floorArray [xx, yy + 1] == tileType.VOID) {
						floorArray [xx, yy] = tileType.VOID;
					}
					if (floorArray [xx - 1, yy] == tileType.VOID && floorArray [xx + 1, yy] == tileType.FLOOR && floorArray [xx, yy - 1] == tileType.VOID && floorArray [xx, yy + 1] == tileType.VOID) {
						floorArray [xx, yy] = tileType.VOID;
					}
					if (floorArray [xx - 1, yy] == tileType.FLOOR && floorArray [xx + 1, yy] == tileType.VOID && floorArray [xx, yy - 1] == tileType.VOID && floorArray [xx, yy + 1] == tileType.VOID) {
						floorArray [xx, yy] = tileType.VOID;
					}
					//if the block is an island with no other connectors
					if (floorArray [xx - 1, yy] == tileType.VOID && floorArray [xx + 1, yy] == tileType.VOID && floorArray [xx, yy - 1] == tileType.VOID && floorArray [xx, yy + 1] == tileType.VOID) {
						floorArray [xx, yy] = tileType.VOID;
					}
					//If the block is an endcap to a hall way
					if (floorArray [xx - 1, yy] == tileType.VOID && floorArray [xx + 1, yy] == tileType.VOID && floorArray [xx, yy - 1] == tileType.VOID && floorArray [xx, yy + 1] == tileType.ROOM) {
						floorArray [xx, yy] = tileType.VOID;
					}
					if (floorArray [xx - 1, yy] == tileType.VOID && floorArray [xx + 1, yy] == tileType.VOID && floorArray [xx, yy - 1] == tileType.ROOM && floorArray [xx, yy + 1] == tileType.VOID) {
						floorArray [xx, yy] = tileType.VOID;
					}
					if (floorArray [xx - 1, yy] == tileType.VOID && floorArray [xx + 1, yy] == tileType.ROOM && floorArray [xx, yy - 1] == tileType.VOID && floorArray [xx, yy + 1] == tileType.VOID) {
						floorArray [xx, yy] = tileType.VOID;
					}
					if (floorArray [xx - 1, yy] == tileType.ROOM && floorArray [xx + 1, yy] == tileType.VOID && floorArray [xx, yy - 1] == tileType.VOID && floorArray [xx, yy + 1] == tileType.VOID) {
						floorArray [xx, yy] = tileType.VOID;
					}
				}
			}
		}
		paintTheFloor (floorInfo.floorWidth, floorInfo.floorHeight);
		return floorArray;
	}

	void paintTheFloor (int floorWidth, int floorHeight) {
        //Generates Background based upon tile type
        groundMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        for (int xx = 0; xx < floorWidth; xx++) {
			for (int yy = 0; yy < floorHeight; yy++) {
				if (floorArray [xx, yy] == tileType.ROOM) {
					//Instantiate (bgRoom, new Vector3 (xx, yy, 0), Quaternion.identity).transform.SetParent(this.transform.root);
                    groundMap.SetTile(new Vector3Int(xx, yy, 0), floor);  
                }
				if (floorArray [xx, yy] == tileType.FLOOR) {
					//Instantiate (bgFloor, new Vector3 (xx, yy, 0), Quaternion.identity).transform.SetParent(this.transform.root);
                    groundMap.SetTile(new Vector3Int(xx, yy, 0), floor);
                }
				if (floorArray [xx, yy] == tileType.CONNECTION) {
					Instantiate (bgConnection, new Vector3 (xx, yy, 0), Quaternion.identity).transform.SetParent(this.transform.root);
				}
                if (floorArray[xx, yy] == tileType.VOID) {
                    wallMap.SetTile(new Vector3Int(xx, yy, 0), wall);
                }
                if (floorArray[xx, yy] == tileType.SHOPROOM) {//Generate Item Room
                    groundMap.SetTile(new Vector3Int(xx, yy, 0), carpet);
                }
			}	
		}
	}

	bool checkValidRoomGen (int startX, int startY, int roomHeight, int roomWidth) {
		for (int xx = -1; xx < roomHeight + 1; xx++) {
			for (int yy = -1; yy < roomWidth + 1; yy++) {
				if (startX + xx < 0 || startY + yy < 0 || startX + xx > floorArray.GetLength (0) - 1 || startY + yy > floorArray.GetLength (1) - 1) {
					return false; //room ended up going out of bounds
				}
				if (floorArray [startX + xx, startY + yy] == tileType.ROOM || floorArray[startX + xx, startY + yy] == tileType.ITEMROOM || floorArray[startX + xx, startY + yy] == tileType.SHOPROOM) {
					return false; //returns false if the room is overlapping another room
				}
			}
		}
		return true; //returns true if the room is ok to be generated
	}
    public List<GameObject> populateTheFloor(FloorInfo floorInfo) { //Using New Floor Info Object
        List<Vector2> validRoomTiles = new List<Vector2>();
        List<GameObject> spawnedObjects = new List<GameObject>();
        //Gathering all valid room tiles
        for (int xx = 0; xx < floorArray.GetLength(0); xx++) {
            for (int yy = 0; yy < floorArray.GetLength(1); yy++) {
                if (floorArray[xx, yy] == tileType.ROOM) {
                    validRoomTiles.Add(new Vector2(xx, yy));
                }
            }
        }
        //placing the player
        int playerStartPos = Random.Range(0, validRoomTiles.Count);
        GameObject.Find("objPlayer").transform.position = validRoomTiles[playerStartPos];
        GameObject.Find("objPlayer").GetComponent<EntityMovement>().targetPos = GameObject.Find("objPlayer").transform.position;
        validRoomTiles.RemoveAt(playerStartPos);

        //Placing Stairs
        int stairPos = Random.Range(0, validRoomTiles.Count);
        GameObject newStairs = Instantiate(stairs, validRoomTiles[stairPos], Quaternion.identity);
        validRoomTiles.RemoveAt(stairPos);
        spawnedObjects.Add(newStairs);

        //Generating Enemies
        for (int i = 0; i < validRoomTiles.Count / 20; i++) {
            int enemyPos = Random.Range(0, validRoomTiles.Count);
            GameObject enemyToSpawn = floorInfo.enemyTable[GlobalFunc.ReturnIndexFromWeightedTable(Random.Range(0, floorInfo.TotalEnemyWeight), floorInfo.enemySpawnWeight)];
            GameObject newEnemy = Instantiate(enemyToSpawn, validRoomTiles[enemyPos], Quaternion.identity);
            newEnemy.GetComponent<EntityMovement>().targetPos = newEnemy.transform.position;
            validRoomTiles.RemoveAt(enemyPos);
            spawnedObjects.Add(newEnemy);
            //Spawn Awake or Not
            if (Random.Range(0, 100) < 40) { newEnemy.GetComponent<EntityMovement>().isSleeping = false; } else { newEnemy.GetComponent<EntityMovement>().isSleeping = true; }
        }
        //Placing Loot
        for (int i = 0; i < validRoomTiles.Count / 20; i++) {
            //Choose Loot Position
            int lootPos = Random.Range(0, validRoomTiles.Count);
            //Choose Loot to Spawn
            Item itemToSpawn = floorInfo.itemTable[GlobalFunc.ReturnIndexFromWeightedTable(Random.Range(0, floorInfo.TotalItemWeight), floorInfo.itemSpawnWeight)];
            GameObject newLoot = Instantiate(itemPrefab, validRoomTiles[lootPos], Quaternion.identity);
            newLoot.GetComponent<ItemScript>().setUp(itemToSpawn);
            //newLoot.GetComponent<EnemyMovement>().targetPos = newLoot.transform.position;
            validRoomTiles.RemoveAt(lootPos);
            spawnedObjects.Add(newLoot);
        }

        //Place Shop Items
        if (!floorInfo.noShop) {
            List<Vector2> shopTiles = GatherShopTiles();
            Item itemToSpawn = floorInfo.itemTable[GlobalFunc.ReturnIndexFromWeightedTable(Random.Range(0, floorInfo.TotalItemWeight), floorInfo.itemSpawnWeight)];
            GameObject newLoot = Instantiate(itemPrefab, shopTiles[6], Quaternion.identity);
            newLoot.GetComponent<ItemScript>().setUp(itemToSpawn);
            newLoot.gameObject.GetComponent<ItemScript>().makeBuyable();

            itemToSpawn = floorInfo.itemTable[GlobalFunc.ReturnIndexFromWeightedTable(Random.Range(0, floorInfo.TotalItemWeight), floorInfo.itemSpawnWeight)];
            newLoot = Instantiate(itemPrefab, shopTiles[8], Quaternion.identity);
            newLoot.GetComponent<ItemScript>().setUp(itemToSpawn);
            newLoot.gameObject.GetComponent<ItemScript>().makeBuyable();
            
            itemToSpawn = floorInfo.itemTable[GlobalFunc.ReturnIndexFromWeightedTable(Random.Range(0, floorInfo.TotalItemWeight), floorInfo.itemSpawnWeight)];
            newLoot = Instantiate(itemPrefab, shopTiles[16], Quaternion.identity);
            newLoot.GetComponent<ItemScript>().setUp(itemToSpawn);
            newLoot.gameObject.GetComponent<ItemScript>().makeBuyable();

            itemToSpawn = floorInfo.itemTable[GlobalFunc.ReturnIndexFromWeightedTable(Random.Range(0, floorInfo.TotalItemWeight), floorInfo.itemSpawnWeight)];
            newLoot = Instantiate(itemPrefab, shopTiles[18], Quaternion.identity);
            newLoot.GetComponent<ItemScript>().setUp(itemToSpawn);
            newLoot.gameObject.GetComponent<ItemScript>().makeBuyable();


        }

        return spawnedObjects;
    }

    List<Vector2> GatherShopTiles() {
        List<Vector2> shopTiles = new List<Vector2>();
        for (int xx = 0; xx < floorArray.GetLength(0); xx++) {
            for (int yy = 0; yy < floorArray.GetLength(1); yy++) {
                if (floorArray[xx, yy] == tileType.SHOPROOM) {
                    shopTiles.Add(new Vector2(xx, yy));
                }
            }
        }

        return shopTiles;
    }



	public void GenerateCorridor (Vector2 startingPos) {
		if(floorArray[(int)startingPos.x,(int)startingPos.y] != tileType.VOID){
			return;
		}
		Corridor corridorTilePrime = new Corridor (startingPos);
		List<Corridor> hallwayList = new List<Corridor> ();
		Vector2 newPos;
		hallwayList.Add (corridorTilePrime);
        string prevDir="";
		while(hallwayList.Count > 0) {
			if (hallwayList.Count != 0) {
				if (floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y] == tileType.VOID) {
					floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y] = tileType.FLOOR;
				}
				List<string> directions = new List<string> ();
				if (hallwayList [hallwayList.Count - 1].position.x - 1 > 0 && floorArray [(int)hallwayList [hallwayList.Count - 1].position.x - 1, (int)hallwayList [hallwayList.Count - 1].position.y] == tileType.VOID) {
					if(floorArray [(int)hallwayList [hallwayList.Count - 1].position.x - 2, (int)hallwayList [hallwayList.Count - 1].position.y] == tileType.VOID){
						directions.Add ("W");
					}
				}
				if (hallwayList [hallwayList.Count - 1].position.x + 1 < floorArray.GetLength(0)-1 && floorArray [(int)hallwayList [hallwayList.Count - 1].position.x + 1, (int)hallwayList [hallwayList.Count - 1].position.y] == tileType.VOID) {
					if(floorArray [(int)hallwayList [hallwayList.Count - 1].position.x + 2, (int)hallwayList [hallwayList.Count - 1].position.y] == tileType.VOID) {	
						directions.Add ("E");
					}
				}
				if (hallwayList [hallwayList.Count - 1].position.y - 1 > 0 && floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y - 1] == tileType.VOID) {
					if(floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y - 2] == tileType.VOID) {	
						directions.Add ("S");
					}
				}
				if (hallwayList [hallwayList.Count - 1].position.y + 1 < floorArray.GetLength(1)-1 && floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y + 1] == tileType.VOID) {
					if(floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y + 2] == tileType.VOID) {	
						directions.Add ("N");
					}
				}
				if (directions.Count > 0) {
                    string direction;
                    if (directions.Contains(prevDir) && Random.Range(0, 100) < 60) {
                        direction = prevDir;
                    }
                    else {
                        direction = directions[Random.Range(0, directions.Count)];
                    }
					switch (direction) {
					case "N":
						newPos = new Vector2 (hallwayList [hallwayList.Count - 1].position.x, hallwayList [hallwayList.Count - 1].position.y + 1);
						hallwayList.Add (new Corridor (newPos));
						if (floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y] == tileType.VOID) {
							floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y] = tileType.FLOOR;
						}
						newPos = new Vector2 (hallwayList [hallwayList.Count - 1].position.x, hallwayList [hallwayList.Count - 1].position.y + 1);
						hallwayList.Add (new Corridor (newPos));
                        prevDir = "N";
						break;
					case "E":
						newPos = new Vector2 (hallwayList [hallwayList.Count - 1].position.x + 1, hallwayList [hallwayList.Count - 1].position.y);
						hallwayList.Add (new Corridor (newPos));
						if (floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y] == tileType.VOID) {
							floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y] = tileType.FLOOR;
						}
						newPos = new Vector2 (hallwayList [hallwayList.Count - 1].position.x + 1, hallwayList [hallwayList.Count - 1].position.y);
						hallwayList.Add (new Corridor (newPos));
                            prevDir = "E";
						break;
					case "S":
						newPos = new Vector2 (hallwayList [hallwayList.Count - 1].position.x, hallwayList [hallwayList.Count - 1].position.y - 1);
						hallwayList.Add (new Corridor (newPos));
						if (floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y] == tileType.VOID) {
							floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y] = tileType.FLOOR;
						}
						newPos = new Vector2 (hallwayList [hallwayList.Count - 1].position.x, hallwayList [hallwayList.Count - 1].position.y - 1);
						hallwayList.Add (new Corridor (newPos));
                            prevDir = "S";
						break;
					case "W":
						newPos = new Vector2 (hallwayList [hallwayList.Count - 1].position.x - 1, hallwayList [hallwayList.Count - 1].position.y);
						hallwayList.Add (new Corridor (newPos));
						if (floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y] == tileType.VOID) {
							floorArray [(int)hallwayList [hallwayList.Count - 1].position.x, (int)hallwayList [hallwayList.Count - 1].position.y] = tileType.FLOOR;
						}
						newPos = new Vector2 (hallwayList [hallwayList.Count - 1].position.x - 1, hallwayList [hallwayList.Count - 1].position.y);
						hallwayList.Add (new Corridor (newPos));
                            prevDir = "S";
						break;
					default:
						break;
					}
					direction = "";
				} else {
					if(hallwayList.Count > 1){
						hallwayList.RemoveRange(hallwayList.Count - 2,2);
					}else{
						hallwayList.RemoveAt (hallwayList.Count - 1);
					}
				}
				directions.Clear ();
			}
		}
	}

    bool GenerateRoom(int roomWidth,int roomHeight, tileType roomType) {
        
            int row1 = Random.Range(1, floorArray.GetLength(0) - roomHeight - 2) / 2 * 2 + 1;
            int col1 = Random.Range(1, floorArray.GetLength(1) - roomWidth - 2) / 2 * 2 + 1;
            if (checkValidRoomGen(col1, row1, roomHeight, roomWidth)) {
                for (int xx = 0; xx < roomHeight; xx++) {
                    for (int yy = 0; yy < roomWidth; yy++) {
                        floorArray[col1 + xx, row1 + yy] = roomType;
                    }
                }
            return true;
            }
        return false;
        }
}
