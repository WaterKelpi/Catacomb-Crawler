using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewFloorInfo",menuName = "Floor Info")]
public class FloorInfo : ScriptableObject
{
    public List<GameObject> enemyTable;
    public List<Item> itemTable;
    public List<int> enemySpawnWeight, itemSpawnWeight;

    public int floorWidth, floorHeight, roomMin, roomMax;

    public bool noShop, noItemRoom;

    public int TotalItemWeight { get {
            int totalWeight = 0;
            if (itemSpawnWeight.Count == 0) {
                return totalWeight;
            }
            else {
                foreach (int itemWeight in itemSpawnWeight) {
                    totalWeight += itemWeight;
                }
                return totalWeight;
            }
        } }
    public int TotalEnemyWeight {
        get {
            int totalWeight = 0;
            if (enemySpawnWeight.Count == 0) {
                return totalWeight;
            }
            else {
                foreach (int enemyWeight in enemySpawnWeight) {
                    totalWeight += enemyWeight;
                }
                return totalWeight;
            }
        }
    }
}
