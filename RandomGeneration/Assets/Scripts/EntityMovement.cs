using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    public bool isMoving, canMove, playerControlled,isSleeping;
    public Vector2 curPos, targetPos;
    public Transform thisEntity, agroObj;
    public float moveSpeed = 5;
    public float attackRange;

    GameObject gm;
    FloorManager fm;
    TurnHandling th;
    AdventureMenuHandler amh;

    EntityManager eManager;


    public PathRequestManager pathManager;
    public Vector2[] directions;
    public Vector2[] path;
    int targetIndex;
    public GameObject unarmedAttack;
    [SerializeField]
    private bool curTurn;
    public bool CurTurn { get { return curTurn; } }
    private entityDirection curDir;
    public entityDirection CurDirection { get { return curDir; } }

    private void Awake() {
        gm = GameObject.Find("Game Manager");
        fm = gm.GetComponent<FloorManager>();
        th = gm.GetComponent<TurnHandling>();
        amh = gm.GetComponent<AdventureMenuHandler>();
        eManager = GetComponent<EntityManager>();
        thisEntity = transform;
        pathManager = gm.GetComponent<PathRequestManager>();
    }


    // Start is called before the first frame update
    void Start(){
        curPos = (Vector2)thisEntity.position;
        targetPos = curPos;
    }

    // Update is called once per frame
    void Update(){
        curPos = (Vector2)thisEntity.position;

        if (!playerControlled) {
            if (agroObj == null && Vector2.Distance(GameObject.Find("objPlayer").transform.position, transform.position) < 8f && !isSleeping) {//Agro on Player if line of site
                Debug.DrawRay(transform.position, (GameObject.Find("objPlayer").transform.position - transform.position));
                if (agroObj == null && Physics2D.CircleCast(transform.position, .45f, (GameObject.Find("objPlayer").transform.position - transform.position)) == GameObject.Find("objPlayer").transform) {
                    agroObj = GameObject.Find("objPlayer").transform;
                }
            }
            if (agroObj != null && Physics2D.CircleCast(transform.position, .45f, (GameObject.Find("objPlayer").transform.position - transform.position)).transform != GameObject.Find("objPlayer").transform
                && Vector2.Distance(transform.position, GameObject.Find("objPlayer").transform.position) > 5) {
                agroObj = null;
                path = new Vector2[0];
            }
        }



        if (!amh.IsPaused) {
            UpdateAnimationVariables();
            if (isMoving) {
                if (GetComponent<SpriteRenderer>().isVisible) { thisEntity.position = Vector2.MoveTowards((Vector2)thisEntity.position, targetPos, Time.deltaTime * moveSpeed);}
                else { thisEntity.position = targetPos; EndTurn(); return; }
                if (curPos == targetPos) {
                    EndTurn();
                }
                return;
            }
            else {
                if (canMove) {
                    //PC Movement
                    if (playerControlled) {
                        MoveTargetPos(new Vector2(Mathf.Round(Input.GetAxisRaw("Horizontal")), Mathf.Round(Input.GetAxisRaw("Vertical"))));
                        if (Input.GetButtonDown("RightButton") && !isMoving && canMove) {
                            canMove = false;
                            Debug.Log("ATTACK!");
                            if (!(eManager.EntityArmor[7] == null ? unarmedAttack : eManager.EntityArmor[7].weaponPrefab).GetComponent<WeaponHandler>().IsThrown) {
                                GameObject newAttack;
                                switch (curDir) {
                                    case entityDirection.N:

                                        newAttack = Instantiate(eManager.EntityArmor[7] == null ? unarmedAttack : eManager.EntityArmor[7].weaponPrefab, ((Vector2)transform.position +
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetVert : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetVert)), Quaternion.identity);
                                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, 90); }
                                        break;
                                    case entityDirection.NE:
                                        newAttack = Instantiate(eManager.EntityArmor[7] == null ? unarmedAttack : eManager.EntityArmor[7].weaponPrefab, ((Vector2)transform.position +
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetVert : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetVert) +
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetHorz : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetHorz)), Quaternion.identity);
                                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, 45); }
                                        break;
                                    case entityDirection.E:
                                        newAttack = Instantiate(eManager.EntityArmor[7] == null ? unarmedAttack : eManager.EntityArmor[7].weaponPrefab, ((Vector2)transform.position +
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetHorz : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetHorz)), Quaternion.identity);
                                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                        break;
                                    case entityDirection.SE:
                                        newAttack = Instantiate(eManager.EntityArmor[7] == null ? unarmedAttack : eManager.EntityArmor[7].weaponPrefab, ((Vector2)transform.position -
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetVert : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetVert) +
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetHorz : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetHorz)), Quaternion.identity);
                                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, -45); }
                                        break;
                                    case entityDirection.S:
                                        newAttack = Instantiate(eManager.EntityArmor[7] == null ? unarmedAttack : eManager.EntityArmor[7].weaponPrefab, ((Vector2)transform.position -
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetVert : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetVert)), Quaternion.identity);
                                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, -90); }
                                        break;
                                    case entityDirection.SW:
                                        newAttack = Instantiate(eManager.EntityArmor[7] == null ? unarmedAttack : eManager.EntityArmor[7].weaponPrefab, ((Vector2)transform.position -
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetVert : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetVert) -
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetHorz : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetHorz)), Quaternion.identity);
                                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, -135); }
                                        break;
                                    case entityDirection.W:
                                        newAttack = Instantiate(eManager.EntityArmor[7] == null ? unarmedAttack : eManager.EntityArmor[7].weaponPrefab, ((Vector2)transform.position -
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetHorz : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetHorz)), Quaternion.identity);
                                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                        break;
                                    case entityDirection.NW:
                                        newAttack = Instantiate(eManager.EntityArmor[7] == null ? unarmedAttack : eManager.EntityArmor[7].weaponPrefab, ((Vector2)transform.position +
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetVert : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetVert) -
                                            (eManager.EntityArmor[7] == null ? unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetHorz : eManager.EntityArmor[7].weaponPrefab.GetComponent<WeaponHandler>().WeaponOffsetHorz)), Quaternion.identity);
                                        newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                        if (newAttack.GetComponent<WeaponHandler>().NeedsRotate) { newAttack.transform.eulerAngles = new Vector3(0, 0, 135); }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else {
                                eManager.ThrowItem(eManager.EntityArmor[7] == null ? unarmedAttack : eManager.EntityArmor[7].weaponPrefab, CurDirection);
                            }

                        }





                        

                    }
                    //NPC Movement
                    else {
                        if (agroObj == null && Vector2.Distance(GameObject.Find("objPlayer").transform.position, transform.position) < 1.5f || isSleeping && Vector2.Distance(GameObject.Find("objPlayer").transform.position, transform.position) < 1.5f) {
                            agroObj = GameObject.Find("objPlayer").transform;
                            isSleeping = false;
                            EndTurn();
                            return;
                        }
                        if (isSleeping) {
                            EndTurn();
                            return;
                        }
                        if (agroObj != null) {
                            PathRequestManager.RequestPath(transform.position, agroObj.position, OnPathFound);
                        }

                        //aDebug.Log(path.Length);
                        if (path.Length == 1 || agroObj!= null && Vector2.Distance(transform.position,agroObj.position) <= attackRange && GlobalFunc.HasLineOfSight(this.gameObject,agroObj.gameObject,attackRange,GlobalFunc.entityDirectionToVector2(curDir))) {
                            if (!isMoving && canMove) {
                                canMove = false;
                                curDir = GlobalFunc.GetDirection(path[0] - curPos);
                                Debug.Log("ATTACK!");
                                if (!unarmedAttack.GetComponent<WeaponHandler>().IsThrown) {
                                    GameObject newAttack;
                                    
                                    switch (curDir) {
                                        case entityDirection.N:
                                            newAttack = Instantiate(unarmedAttack, ((Vector2)transform.position + unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetVert), Quaternion.identity);
                                            newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                            break;
                                        case entityDirection.E:
                                            newAttack = Instantiate(unarmedAttack, ((Vector2)transform.position + unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetHorz), Quaternion.identity);
                                            newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                            break;
                                        case entityDirection.S:
                                            newAttack = Instantiate(unarmedAttack, ((Vector2)transform.position - unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetVert), Quaternion.identity);
                                            newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                            break;
                                        case entityDirection.W:
                                            newAttack = Instantiate(unarmedAttack, ((Vector2)transform.position - unarmedAttack.GetComponent<WeaponHandler>().WeaponOffsetHorz), Quaternion.identity);
                                            newAttack.GetComponent<WeaponHandler>().parentObject = this.gameObject;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else {
                                    eManager.ThrowItem(unarmedAttack,curDir);
                                }
                                
                            }
                            return;
                        }
                        else if (path.Length != 0) {
                            curDir = GlobalFunc.GetDirection(path[0] - curPos);
                            MoveTargetPos(path[0] - curPos);
                        }
                        else {
                            curDir = GlobalFunc.getRandDirection();
                            MoveTargetPos(GlobalFunc.entityDirectionToVector2(curDir));
                            isMoving = true;
                        }
                    }
                }
                
            }

        }

    }

    void UpdateAnimationVariables() {
        if (curTurn) {
            if (playerControlled) { curDir = !isMoving ? GlobalFunc.GetDirection(Mathf.Round(Input.GetAxisRaw("Horizontal")), Mathf.Round(Input.GetAxisRaw("Vertical")), curDir) : curDir; GetComponent<EntityAnimation>().prevDir = curDir; }
            else { GetComponent<EntityAnimation>().prevDir = curDir; }
        }
        GetComponent<EntityAnimation>().isMoving = isMoving;
        GetComponent<EntityAnimation>().isSleeping = isSleeping;
    }

    public void EndTurn() {
        if (playerControlled) {
            if (eManager.curBelly > 0) {
                eManager.regenCounter++;
            }
            else {
                GetComponent<EntityStatHandler>().TakeFlatDamage(1);
                eManager.regenCounter = 0;
            }
            if (eManager.regenCounter > 9) {
                GetComponent<EntityStatHandler>().curHP += GetComponent<EntityStatHandler>().curHP < GetComponent<EntityStatHandler>().MaxHP ? 1 : 0;
            }
            eManager.hungerCounter += eManager.curBelly > 0 ? 1 : 0;
            if (eManager.hungerCounter > 9) {
                eManager.curBelly -= eManager.curBelly > 0 ? 1 : 0;
                eManager.hungerCounter = 0;
            }
        }

        isMoving = false;
        canMove = false;
        th.EndTurn();
        curTurn = false;
    }

    void MoveTargetPos(Vector2 direction) {
        if (direction != Vector2.zero) {
            if (!Input.GetButton("LeftButton")) {//Holding not Still
                if (!Physics2D.CircleCast(curPos, .4f, direction, 1)) {
                    if(!Input.GetButton("TopButton") || Input.GetButton("TopButton") && Mathf.Abs(direction.magnitude) > 1) { targetPos += direction; }
                    if (curPos != targetPos) {
                        isMoving = true;
                    }
                }

            }
            else {
                targetPos += direction;
                UpdateAnimationVariables();
                targetPos = curPos;
            }
        }
        if (!playerControlled) { isMoving = true; }
    }


    //If it has pathfinding
    public void OnPathFound(Vector2[] newPath, bool pathSuccessful) {
        if (pathSuccessful) {
            path = newPath;
            targetIndex = 0;
        }
    }

    IEnumerator FollowPath() {
        if (path.Length > 0) {
            Vector2 currentWaypoint = path[0];
            while (true) {
                if (Vector2.Distance((Vector2)transform.position, currentWaypoint) < .25f) {
                    targetIndex++;
                    if (targetIndex >= path.Length) {
                        targetIndex = 0;
                        path = new Vector2[0];
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }
                //GetComponent<Rigidbody2D>().velocity = (currentWaypoint - (Vector2)transform.position).normalized;
                yield return null;
            }
        }
    }

    public void StartTurn() {
        if (!curTurn) {
            canMove = true;
            curTurn = true;
        }
        else {
            return;
        }
    }


    public void OnDrawGizmos() {

        if (path != null) {
            for (int i = targetIndex; i < path.Length; i++) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], new Vector3(.5f, .5f, .5f));

                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }

    }


    public void MoveOffScreen() {
        curPos = (Vector2)thisEntity.position;
        if (agroObj != null) {
            PathRequestManager.RequestPath(transform.position, agroObj.position, OnPathFound);
        }
        if (path.Length != 0) {
            curDir = GlobalFunc.GetDirection(path[0] - curPos);
            MoveTargetPos(path[0] - curPos);
        }
        else {
            curDir = GlobalFunc.getRandDirection();
            MoveTargetPos(GlobalFunc.entityDirectionToVector2(curDir));
        }

        thisEntity.position = targetPos;

    }

}
