﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    int inventorySize, gold;

    public int Gold {
        get { return gold; }
    }

    [SerializeField]
    List<Item> inventory;
    public List<Item> Inventory {
        get { return inventory; }
    }
    [SerializeField]
    List<int> inventoryCount;
    public List<int> InventoryCount {
        get { return inventoryCount; }
    }
    [SerializeField]
    List<bool> inventoryEquip;
    public List<bool> InventoryEquip {
        get { return inventoryEquip; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AddItem(Item item) { //Returns true if item was added
        if (item.itemType == itemType.gold) {
            gold += item.numInStack;
            return true;
        }
        if (inventory.Count < inventorySize) {
            if (item.maxStack == 1) {
                inventory.Add(item);
                inventoryCount.Add(1);
                inventoryEquip.Add(false);
                return true;
            }else {
                for (int i = 0; i < inventory.Count; i++) {
                    if (inventory[i] == item && inventoryCount[i] < item.maxStack) { //Found the item, and can hold more in that slot
                        inventoryCount[i]+=item.numInStack;
                        if (inventoryCount[i] > item.maxStack) {//item overflows the stack
                            Debug.Log("I'm holding too many for the stack");
                            inventory.Add(item);
                            inventoryCount.Add(inventoryCount[i]%item.maxStack);
                            Debug.Log("Adding" + (inventoryCount[i] % item.maxStack) + "to the new stack");
                            inventoryCount[i] = item.maxStack;
                        }
                        return true;
                    }
                }
                inventory.Add(item);
                inventoryCount.Add(item.numInStack);
                inventoryEquip.Add(false);
                if (inventoryCount[inventoryCount.Count-1] > item.maxStack) {//item overflows the stack
                    int curIndex = inventoryCount.Count-1;
                    Debug.Log(curIndex);
                    inventory.Add(item);
                    inventoryCount.Add(inventoryCount[curIndex] % item.maxStack);
                    inventoryEquip.Add(false);
                    Debug.Log("Adding" + (inventoryCount[curIndex] % item.maxStack) + "to the new stack");
                    inventoryCount[curIndex] = item.maxStack;
                }
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(int itemIndex, int numToRemove) {
        if (numToRemove <= inventoryCount[itemIndex]) {
            inventoryCount[itemIndex] -= numToRemove;
            if (inventoryCount[itemIndex] == 0) {
                inventory.RemoveAt(itemIndex);
                inventoryCount.RemoveAt(itemIndex);
            }
            return true;
        }
        return false;
    }
    /*
    public bool EquipItem(int itemIndex, bool targetState) {
        if (inventoryEquip[itemIndex] != targetState) {
            inventoryEquip[itemIndex] = targetState;
            return true;
        }
        else {
            return false;
        }
    }*/
    public bool GainGold(int amtToGain) {
        if (gold + amtToGain > 0) { gold += amtToGain; return true; }
        return false;
    }
}
