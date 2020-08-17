using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour {
	public FloorGeneration fg;
	public tileType[,] curFloor;
	public int floorW,floorH,minRoomSize,maxRoomSize;
	public List<GameObject> enemies,loot,spawnedObjects;
    public FloorInfo floorInfo;
    GridHandler gh;

    private int floorNum = 1;
    public int FloorNum {
        get { return floorNum; }
    }

    TurnHandling th;

	void Awake(){
        th = GetComponent<TurnHandling>();
        gh = GetComponent<GridHandler>();
		curFloor = fg.floorGen(floorInfo);
        spawnedObjects = fg.populateTheFloor(floorInfo);
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
        curFloor = fg.floorGen(floorInfo);
        spawnedObjects = fg.populateTheFloor(floorInfo);
        th.CollectActors();
        gh.CreateGrid();
        foreach (GameObject actor in th.actorTurnOrder) {
            if (actor.tag == "Enemy") {
                actor.GetComponent<EntityStatHandler>().GainLevel(Mathf.CeilToInt(floorNum * Random.Range(.4f, 1f)));
            }
        }
    }
}
