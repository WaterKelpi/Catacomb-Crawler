using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum tileType {
    VOID,
    FLOOR,
    WALL,
    ROOM,
    ITEMROOM,
    CONNECTION,
    SHOPROOM
};

public enum entityDirection { N,NE, E, SE,S,SW,W,NW};

public enum growthSpeeds { fast=11,medium=10,slow=9};
public enum statProficiency {fantastic=12,good=11,average=10,bad=9,ass=8};

public enum itemType {
    gold,
    equipment,
    edible,
    usable,
    chest,
    key
}

public enum equipType {
    head,
    chest,
    arms,
    legs,
    feet,
    acc1,
    acc2,
    weapon
}

public enum menuType {
    paused,
    moves,
    inventory,
    stats,
    others,
    ground,
    rest
}

public enum statType {
strength,
dexterity,
inteligence,
defense
}