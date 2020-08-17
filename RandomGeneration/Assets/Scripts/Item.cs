using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewItem",menuName ="New Item")]



public class Item : ScriptableObject
{
    public itemType itemType;
    public string itemName;
    public int maxStack;
    public int numInStack;
    public Sprite itemSprite;
    public bool consumable, throwable, useable, equipable;
    public int bellyChange, healthChange, thrownDamage;
    public int[] statBoosts = new int[5]; //HP,STR,DEX,INTL,DEF
    public float speedBoost;
    public equipType equipType = equipType.acc1;
    public GameObject weaponPrefab;
    public int baseBuyAmt;
}
