
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(EntityStatHandler), typeof(InventoryManager), typeof(EntityMovement))]

public class EntityManager : MonoBehaviour {
    AdventureMenuHandler aMH;
    public bool isMoving;
    public Vector2 targetPos;
    [SerializeField]
    GameObject itemPrefab;
    EntityStatHandler statHandler;
    public bool waiting;
    public float curEnergy;
    public float speed = 1; //Speed is used in energy calculation
    public EntityStatHandler StatHandler {
        get { return statHandler; }
    }
    InventoryManager inventory;
    public InventoryManager Inventory {
        get { return inventory; }
    }

    bool playerControlled;

    private GameObject lastHitBy;

    public int regenCounter = 0;
    public int hungerCounter = 0;

    public int curBelly, maxBelly;

    [SerializeField]
    Item[] entityArmor = new Item[8]; //Head,Chest,Hands,Legs,Feet,Acc1,Acc2,Weapon
    public Item[] EntityArmor { get { return entityArmor; } }

    [SerializeField]
    List<GameObject> specialMoves;
    public List<GameObject> SpecialMoves { get { return specialMoves; } }

    GameObject objOntopOf;
    public GameObject ObjOntopOf {get {return objOntopOf;}}

    private void Awake() {
        statHandler = GetComponent<EntityStatHandler>();
        inventory = GetComponent<InventoryManager>();
        aMH = GameObject.Find("Game Manager").GetComponent<AdventureMenuHandler>();
        playerControlled = GetComponent<EntityMovement>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (statHandler.curHP < 0) { Die(); }
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.GetComponent<WeaponHandler>() != null) {
            WeaponHandler weapon = collision.GetComponent<WeaponHandler>();
            if (weapon.IsThrown) { return; }
            if (weapon.parentObject == this.gameObject){ return; }
            EntityStatHandler statHandler = GetComponent<EntityStatHandler>();
            EntityStatHandler eStatHandler = collision.GetComponent<WeaponHandler>().parentObject.GetComponent<EntityStatHandler>();
            lastHitBy = collision.GetComponent<WeaponHandler>().parentObject;
            switch (weapon.ScaledStat) {
                case statType.strength:
                    statHandler.TakeDamage(weapon.WeaponDamage, eStatHandler.Str, eStatHandler.Level);
                    break;
                case statType.dexterity:
                    statHandler.TakeDamage(weapon.WeaponDamage, eStatHandler.Dex, eStatHandler.Level);
                    break;
                case statType.inteligence:
                    statHandler.TakeDamage(weapon.WeaponDamage, eStatHandler.Intl, eStatHandler.Level);
                    break;
                case statType.defense:
                    statHandler.TakeDamage(weapon.WeaponDamage, eStatHandler.Def, eStatHandler.Level);
                    break;

            }  
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.GetComponent<ItemScript>() != null && Vector3.Distance(collision.transform.position, transform.position) <= .1f && curEnergy >= 4) {//Pick up an item
            if (!collision.GetComponent<ItemScript>().Buyable) {
                if (inventory.AddItem(collision.GetComponent<ItemScript>().Item)) {
                    aMH.NewLogMessage(string.Format("{0} picked up {1}!", statHandler.entityName, collision.GetComponent<ItemScript>().Item.itemName));
                    Destroy(collision.gameObject);
                }
            } else {
                objOntopOf = collision.gameObject;
            }
            
        }
        if (transform.tag == "Player" && collision.tag == "Stairs" && Vector3.Distance(collision.transform.position, transform.position) <= .1f && curEnergy >= 4) {
            Debug.Log("Next Floor!");
            GameObject.Find("Game Manager").GetComponent<FloorManager>().NextFloor();
        }
        if (collision.GetComponent<WeaponHandler>() != null && Vector3.Distance(collision.transform.position, transform.position) <= .1f) {
            WeaponHandler weapon = collision.GetComponent<WeaponHandler>();
            if (!weapon.IsThrown) { return; }
            if (weapon.parentObject == this.gameObject) { return; }
            EntityStatHandler statHandler = GetComponent<EntityStatHandler>();
            EntityStatHandler eStatHandler = collision.GetComponent<WeaponHandler>().parentObject.GetComponent<EntityStatHandler>();
            lastHitBy = collision.GetComponent<WeaponHandler>().parentObject;
            switch (weapon.ScaledStat) {
                case statType.strength:
                    statHandler.TakeDamage(weapon.WeaponDamage, eStatHandler.Str, eStatHandler.Level);
                    break;
                case statType.dexterity:
                    statHandler.TakeDamage(weapon.WeaponDamage, eStatHandler.Dex, eStatHandler.Level);
                    break;
                case statType.inteligence:
                    statHandler.TakeDamage(weapon.WeaponDamage, eStatHandler.Intl, eStatHandler.Level);
                    break;
                case statType.defense:
                    statHandler.TakeDamage(weapon.WeaponDamage, eStatHandler.Def, eStatHandler.Level);
                    break;
            }
            weapon.AttackFinished();
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (objOntopOf == collision.gameObject) { objOntopOf = null; }
    }

    void Die() {
        if (lastHitBy != null) {
            lastHitBy.GetComponent<EntityStatHandler>().GainExp(statHandler.ExpYield);
            aMH.NewLogMessage(string.Format("{0} was felled!", statHandler.entityName));
        }

        foreach (Item item in inventory.Inventory) {
            GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            newItem.GetComponent<ItemScript>().setUp(item);
        }

        //Remove Entity From Queue
        TurnHandling th = GameObject.Find("Game Manager").GetComponent<TurnHandling>();
        th.actorTurnOrder.Remove(this.gameObject);
        th.ValidateCurActor();


        Destroy(this.gameObject);
    }

    public bool ConsumeItem(int itemIndex) {
        Item itemToConsume = inventory.Inventory[itemIndex];
        if (inventory.RemoveItem(itemIndex, 1)) {
            curBelly += itemToConsume.bellyChange;
            statHandler.TakeFlatDamage(itemToConsume.healthChange * -1);
            return true;
        }
        return false;
    }

    public bool EquipItem(int itemIndex) {
        Item itemToEquip = inventory.Inventory[itemIndex];
        if (inventory.RemoveItem(itemIndex, 1)){
            switch (itemToEquip.equipType) {
                case equipType.head:
                    if (entityArmor[0] != null) { UnequipItem(0);}
                    entityArmor[0] = itemToEquip;
                    break;
                case equipType.weapon:
                    if (entityArmor[7] != null) { UnequipItem(7); }
                    entityArmor[7] = itemToEquip;
                    break;
            }
            statHandler.UpdateStats();
            speed += itemToEquip.speedBoost;
            return true;
        }
        return false;

    }

    public bool UnequipItem(int armorSlot) {
        if (entityArmor[armorSlot] != null) {
            Item itemToUnequip = entityArmor[armorSlot];
            inventory.AddItem(entityArmor[armorSlot]);
            statHandler.UpdateStats();
            speed -= itemToUnequip.speedBoost;
            entityArmor[armorSlot] = null;
        }
        return false;
    }

    public bool ThrowItem(int itemIndex, entityDirection directionThrown) {
        Item itemToThrow = inventory.Inventory[itemIndex];
        if (inventory.RemoveItem(itemIndex, 1)) {
            GameObject thrownItem = Instantiate(itemToThrow.weaponPrefab, transform.position, Quaternion.identity);
            thrownItem.GetComponent<WeaponHandler>().parentObject = this.gameObject;
            thrownItem.GetComponent<Rigidbody2D>().velocity = GlobalFunc.entityDirectionToVector2(directionThrown).normalized * 5;
            if (thrownItem.GetComponent<WeaponHandler>().NeedsRotate) {
                switch (directionThrown) {
                    case entityDirection.N:
                        thrownItem.transform.eulerAngles = new Vector3(0, 0, 90);
                        break;
                    case entityDirection.E:
                        break;
                    case entityDirection.S:
                        thrownItem.transform.eulerAngles = new Vector3(0, 0, -90);
                        break;
                    case entityDirection.W:
                        thrownItem.transform.eulerAngles = new Vector3(0, 0, 180);
                        break;
                }
            }
            
            return true;
        }
        
        return false;
    }

    public bool ThrowItem(GameObject objectThrown, entityDirection directionThrown) {
        
            GameObject thrownItem = Instantiate(objectThrown, transform.position, Quaternion.identity);
            thrownItem.GetComponent<WeaponHandler>().parentObject = this.gameObject;
            thrownItem.GetComponent<Rigidbody2D>().velocity = GlobalFunc.entityDirectionToVector2(directionThrown).normalized * 5;
        if (thrownItem.GetComponent<WeaponHandler>().NeedsRotate) {
            switch (directionThrown) {
                case entityDirection.N:
                    thrownItem.transform.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case entityDirection.E:
                    break;
                case entityDirection.S:
                    thrownItem.transform.eulerAngles = new Vector3(0, 0, -90);
                    break;
                case entityDirection.W:
                    thrownItem.transform.eulerAngles = new Vector3(0, 0, 180);
                    break;
            }
        }
        return true;
    }
    public bool ScrapItem(int itemIndex) {
        Item itemToScrap = inventory.Inventory[itemIndex];
        if (inventory.RemoveItem(itemIndex, 1)) {
            inventory.GainGold(Mathf.FloorToInt(itemToScrap.baseBuyAmt * Random.Range(.4f, .6f))); //Implement intl scaling somehow
            return true;
        }
        return false;

    }

    public bool UseSMove(int moveIndex, entityDirection moveDir) {
        if (moveIndex < specialMoves.Count && moveIndex >= 0) {
            if (specialMoves[moveIndex] != null) {
                if (specialMoves[moveIndex].GetComponent<WeaponHandler>().IsThrown) {
                    ThrowItem(specialMoves[moveIndex], moveDir);
                    return true;
                }
                GameObject newAttack;
                switch (moveDir) {
                    case entityDirection.N:
                        newAttack = Instantiate(specialMoves[moveIndex], (Vector2)transform.position + specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetVert, Quaternion.identity);
                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, 90); }
                        break;
                    case entityDirection.NE:
                        newAttack = Instantiate(specialMoves[moveIndex], ((Vector2)transform.position +
                            specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetVert +
                            specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetHorz), Quaternion.identity);
                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, 45); }
                        break;
                    case entityDirection.E:
                        newAttack = Instantiate(specialMoves[moveIndex], (Vector2)transform.position + specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetHorz, Quaternion.identity);
                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                        break;
                    case entityDirection.SE:
                        newAttack = Instantiate(specialMoves[moveIndex], ((Vector2)transform.position -
                            specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetVert +
                            specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetHorz), Quaternion.identity);
                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, -45); }
                        break;
                    case entityDirection.S:
                        newAttack = Instantiate(specialMoves[moveIndex], (Vector2)transform.position - specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetVert, Quaternion.identity);
                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, -90); }
                        break;
                    case entityDirection.SW:
                        newAttack = Instantiate(specialMoves[moveIndex], ((Vector2)transform.position -
                            specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetVert -
                            specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetHorz), Quaternion.identity);
                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, -135); }
                        break;
                    case entityDirection.W:
                        newAttack = Instantiate(specialMoves[moveIndex], (Vector2)transform.position - specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetHorz, Quaternion.identity);
                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, 180); }
                        break;
                    case entityDirection.NW:
                        newAttack = Instantiate(specialMoves[moveIndex], ((Vector2)transform.position +
                            specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetVert -
                            specialMoves[moveIndex].GetComponent<WeaponHandler>().WeaponOffsetHorz), Quaternion.identity);
                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, 135); }
                        break;
                    default:
                        break;
                }
                if (playerControlled) {
                    curBelly--;
                    curBelly = Mathf.Clamp(curBelly, 0, maxBelly);
                }
                

                return true;
            }
        }

        return false;
    }
}
