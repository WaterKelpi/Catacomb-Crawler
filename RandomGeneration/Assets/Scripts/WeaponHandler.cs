using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WeaponHandler : MonoBehaviour
{
    public GameObject parentObject;
    [SerializeField]
    int weaponDamage;
    public int WeaponDamage { get { return weaponDamage; } }
    [SerializeField]
    bool isThrown;
    public bool IsThrown { get { return isThrown; } }

    [SerializeField]
    statType scaledStat;
    public statType ScaledStat { get { return scaledStat; } }

    [SerializeField]
    bool needsRotate;
    public bool NeedsRotate { get { return needsRotate; } }

    [SerializeField]
    Vector2 weaponOffset; //When facing Right
    public Vector2 WeaponOffsetHorz { get { return weaponOffset; } }
    public Vector2 WeaponOffsetVert { get { return new Vector2(-weaponOffset.y, weaponOffset.x); } }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        if (Vector2.Distance(this.gameObject.transform.position, parentObject.transform.position) > 20) { AttackFinished(); }
        if (GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Wall")) && isThrown) { AttackFinished(); }
    }


    public void AttackFinished() {
        if (parentObject == null) {
            Destroy(this.gameObject);
            return;
        }

        
        parentObject.GetComponent<EntityMovement>().EndTurn();
        
        /*if (parentObject.GetComponent<EnemyMovement>() != null) {
            parentObject.GetComponent<EnemyMovement>().EndTurn();
        }*/
        Destroy(this.gameObject);

    }

}
