using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHandling : MonoBehaviour {

    public EntityActionParser eActionParser;

	public List<GameObject> actorTurnOrder;
	public int curActor;
    public bool hold;
	void Awake(){
		CollectActors();
	}



	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (curActor > actorTurnOrder.Count) { curActor = curActor % actorTurnOrder.Count; }
        while (actorTurnOrder[curActor] == null) {
            actorTurnOrder.RemoveAt(curActor);
            curActor = curActor % actorTurnOrder.Count;
        }
        EntityManager em = actorTurnOrder[curActor].GetComponent<EntityManager>();
        if (em == null) { return; }
        while (em.curEnergy < 4) {
            em.curEnergy += em.speed;
            Debug.Log(em.name + " has " + em.curEnergy + " energy");
            if (em.curEnergy < 4) {
                curActor = (curActor + 1) % actorTurnOrder.Count;
            }
            em = actorTurnOrder[curActor].GetComponent<EntityManager>();
            if (em == null) { return; }
        }
        while (em.curEnergy >= 4 && hold == false) {
            hold = true;
            if (actorTurnOrder[curActor].tag == "Player") {
                actorTurnOrder[curActor].GetComponent<EntityMovement>().StartTurn();
            }
            if (actorTurnOrder[curActor].tag == "Enemy") {
                if (actorTurnOrder[curActor].GetComponent<SpriteRenderer>().isVisible && !actorTurnOrder[curActor].GetComponent<EntityMovement>().canMove) {
                    actorTurnOrder[curActor].GetComponent<EntityMovement>().StartTurn();
                }
                else if (actorTurnOrder[curActor].GetComponent<EntityMovement>().isSleeping) { EndTurn();}
                else { actorTurnOrder[curActor].GetComponent<EntityMovement>().MoveOffScreen(); }

            }
        }





    }



    public void EndTurn() {
        actorTurnOrder[curActor].GetComponent<EntityManager>().curEnergy -= 4;
        curActor = (curActor + 1) % actorTurnOrder.Count;
        hold = false;

    }

	public void CollectActors(){
		actorTurnOrder.Clear();
		actorTurnOrder.Add(GameObject.FindGameObjectWithTag("Player"));
		actorTurnOrder.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        curActor = 0;
	}

    public void ValidateCurActor() {
        curActor = curActor % actorTurnOrder.Count;
    }
}
