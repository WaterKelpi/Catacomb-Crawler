using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour {
	public FloorGeneration fg;
	public tileType[,] curFloor;
	public int floorW,floorH,minRoomSize,maxRoomSize;
	public List<GameObject> enemies,loot,spawnedObjects;
    public FloorInfo[] dungeonFloors;
    public FloorInfo curFloorInfo { get { return dungeonFloors[(floorNum-1)%dungeonFloors.Length]; } }
    GridHandler gh;

    private int floorNum = 1;
    public int FloorNum {
        get { return floorNum; }
    }

    TurnHandling th;

	void Awake(){
        th = GetComponent<TurnHandling>();
        gh = GetComponent<GridHandler>();
		curFloor = fg.floorGen(curFloorInfo);
        spawnedObjects = fg.populateTheFloor(curFloorInfo);
        th.CollectActors();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NextFloor() {
        floorNum++;
        foreach (GameObject obj in spawnedObjects) {
            Destroy(obj);
        }
        curFloor = fg.floorGen(curFloorInfo);
        spawnedObjects = fg.populateTheFloor(curFloorInfo);
        th.CollectActors();
        gh.CreateGrid();
        foreach (GameObject actor in th.actorTurnOrder) {
            if (actor.tag == "Enemy") {
                actor.GetComponent<EntityStatHandler>().GainLevel(Mathf.CeilToInt(floorNum * UnityEngine.Random.Range(.4f, 1f)));
            }
        }
    }
}
