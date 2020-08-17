using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class ItemScript : MonoBehaviour
{
    [SerializeField]
    Item item;
    bool buyable;
    public bool Buyable { get { return buyable; } }

    public Item Item {
        get { return item; }
    }

    public void setUp(Item newItem) {
        item = newItem;
        GetComponent<SpriteRenderer>().sprite = item.itemSprite;
    }

    public void makeBuyable() {
        buyable = true;
    }

    public void makeFree()
    {
        buyable = false;
    }
}
