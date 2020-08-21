using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalFunc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    //Get Direction From Joystick Inputs && Returns Original Direction if no input
    static public entityDirection GetDirection(float horzInput,float vertInput,entityDirection curDir) {
        string dirReturn = "";

        if (vertInput != 0) {
            dirReturn += vertInput > 0 ? "N" : "S";
        }
        if (horzInput != 0) {
            dirReturn += horzInput > 0 ? "E" : "W";
        }

        return dirReturn != "" ?  (entityDirection)Enum.Parse(typeof(entityDirection),dirReturn):curDir;
    }

    //Get Direction From Vector2 Inputs && Returns Original Direction if no input
    static public entityDirection GetDirection(Vector2 direction, entityDirection curDir) {
        string dirReturn = "";

        if (direction.y != 0) {
            dirReturn += direction.y > 0 ? "N" : "S";
        }
        if (direction.x != 0) {
            dirReturn += direction.x > 0 ? "W" : "E";
        }

        return dirReturn != "" ? (entityDirection)Enum.Parse(typeof(entityDirection), dirReturn) : curDir;
    }

    //Get Direction From Joystick Inputs && Returns North if No Inputs
    static public entityDirection GetDirection(Vector2 direction) {
        string dirReturn = "";

        if (direction.y != 0) {
            dirReturn += direction.y > 0 ? "N" : "S";
        }
        if (direction.x != 0) {
            dirReturn += direction.x > 0 ? "E" : "W";
        }

        return dirReturn != "" ? (entityDirection)Enum.Parse(typeof(entityDirection), dirReturn) : entityDirection.N ;
    }

    static public entityDirection getRandDirection() {
        int randDir = UnityEngine.Random.Range(0,8);

        return (entityDirection)randDir;
    }

    static public Vector2 entityDirectionToVector2(entityDirection entityDir) {
        switch (entityDir) {
            case entityDirection.E:return Vector2.right;
            case entityDirection.N:return Vector2.up;
            case entityDirection.S:return Vector2.down;
            case entityDirection.W:return Vector2.left;
            case entityDirection.NE: return Vector2.right + Vector2.up;
            case entityDirection.NW: return Vector2.left+Vector2.up;
            case entityDirection.SE: return Vector2.right+Vector2.down;
            case entityDirection.SW: return Vector2.left+Vector2.down;
            default: return Vector2.zero;
        }
    }

    static public int ReturnIndexFromWeightedTable(int valueOfItem, List<int> weightedValues) {
        int weightTotal = 0;
        for (int itemIndex = 0; itemIndex < GetTotalListValue(weightedValues); itemIndex++) {
            if (valueOfItem >= weightTotal && valueOfItem < (weightTotal + weightedValues[itemIndex])) { return itemIndex; }
            weightTotal += weightedValues[itemIndex];
        }
        return 0;
    }

    static public int GetTotalListValue(List<int> listToTotal) {
        int totalValue = 0;
        if (listToTotal.Count == 0) {
            return totalValue;
        }
        else {
            foreach (int itemValue in listToTotal) {
                totalValue += itemValue;
            }
            return totalValue;
        }
    }
}
