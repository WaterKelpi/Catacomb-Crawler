using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class ItemScript : MonoBehaviour
{
    [SerializeField]
    Item item;
    int numInStack;
    public int NumInStack { get { return numInStack; } }
    bool buyable;
    public bool Buyable { get { return buyable; } }

    public Item Item {
        get { return item; }
    }

    public void setUp(Item newItem) {
        item = newItem;
        GetComponent<SpriteRenderer>().sprite = item.itemSprite;
        numInStack = Mathf.Clamp((int)(item.avgStack * Random.Range(.75f, 1.25f)),1,item.maxStack);
    }

    public void setUp(Item newItem, int howMuchInStack)
    {
        item = newItem;
        GetComponent<SpriteRenderer>().sprite = item.itemSprite;
        numInStack = howMuchInStack;
    }


    public void setUp(Item newItem,bool isBuyable)
    {
        item = newItem;
        GetComponent<SpriteRenderer>().sprite = item.itemSprite;
        numInStack = Mathf.Clamp((int)(item.avgStack * Random.Range(.75f, 1.25f)), 1, item.maxStack);
        buyable = isBuyable;
    }



    public void makeBuyable() {
        buyable = true;
    }

    public void makeFree()
    {
        buyable = false;
    }
}
