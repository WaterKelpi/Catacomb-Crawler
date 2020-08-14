using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class ItemScript : MonoBehaviour
{
    [SerializeField]
    Item item;
    public Item Item {
        get { return item; }
    }

    public void setUp(Item newItem) {
        item = newItem;
        GetComponent<SpriteRenderer>().sprite = item.itemSprite;
    }
}
