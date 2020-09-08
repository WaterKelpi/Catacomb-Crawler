using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;


[CreateAssetMenu(fileName = "NewFloorInfo",menuName = "Floor Info")]
public class FloorInfo : ScriptableObject
{
    public List<GameObject> enemyTable;
    public List<Item> itemTable,shopTable,treasureTable,chestTable;
    public List<int> enemySpawnWeight, itemSpawnWeight, shopSpawnWeight, treasureSpawnWeight,chestSpawnWeight;

    public int floorWidth, floorHeight, roomMin, roomMax;

    public bool noShop, noTreasureRoom;

    public int TotalItemWeight { get { return GlobalFunc.GetTotalListValue(itemSpawnWeight); } }
    public int TotalEnemyWeight {        get {return GlobalFunc.GetTotalListValue(enemySpawnWeight);}    }

    public int TotalShopWeight { get { return GlobalFunc.GetTotalListValue(shopSpawnWeight); } }
    
}
