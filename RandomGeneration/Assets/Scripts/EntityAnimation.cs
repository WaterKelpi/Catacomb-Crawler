using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour
{
    public entityDirection prevDir;
    public bool isMoving, isHurt,isSleeping,meleeAttack,rangedAttack,isAttacking;
    private Animator entityAnimator;
    private void Awake() {
        entityAnimator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        entityAnimator.SetInteger("Direction", (int)prevDir);
        entityAnimator.SetBool("IsMoving", isMoving);
        entityAnimator.SetBool("IsHurt", isHurt);
        entityAnimator.SetBool("IsSleeping", isSleeping);
        entityAnimator.SetBool("IsAttacking", isAttacking);
        entityAnimator.SetBool("MeleeAttack", meleeAttack);
    }

    public void HurtAnimDone() {
        isHurt = false;
    }
}
