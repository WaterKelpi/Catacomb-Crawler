using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityActionParser: MonoBehaviour
{
    public bool ParseAction(string action, string input, EntityManager curEntity) {
        action.ToUpper();
        switch (action) {
            case "WALK":
                entityDirection eDir;
                if (Enum.TryParse<entityDirection>(input, out eDir)) {
                    Walk(eDir, curEntity);
                }
                
                break;
            default:
                return false;

        }





        return false;
    }




    static bool Walk(entityDirection eDir,EntityManager eManager) {
        Vector2 dirToMove = Vector2.zero;
        switch (eDir) {
            case entityDirection.N:
                dirToMove += Vector2.up;
                break;
            case entityDirection.NE:
                dirToMove += Vector2.up;
                dirToMove += Vector2.right;
                break;
            case entityDirection.E:
                dirToMove += Vector2.right;
                break;
            case entityDirection.SE:
                dirToMove += Vector2.down;
                dirToMove += Vector2.right;
                break;
            case entityDirection.S:
                dirToMove += Vector2.down;
                break;
            case entityDirection.SW:
                dirToMove += Vector2.down;
                dirToMove += Vector2.left;
                break;
            case entityDirection.W:
                dirToMove += Vector2.left;
                break;
            case entityDirection.NW:
                dirToMove += Vector2.up;
                break;
        }

        if (!Physics2D.CircleCast(eManager.transform.position, .4f, dirToMove, 1) && !eManager.isMoving) {
            eManager.targetPos += dirToMove;
            eManager.isMoving = true;
        }

        if ((Vector2)eManager.transform.position != eManager.targetPos) {
            eManager.transform.position = Vector2.MoveTowards(new Vector2(eManager.transform.position.x, eManager.transform.position.y), eManager.targetPos, Time.deltaTime);
            return true;
        }
        else {
            return false;
        }
    }



}