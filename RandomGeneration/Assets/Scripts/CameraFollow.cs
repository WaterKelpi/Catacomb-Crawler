using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	public GameObject objToFollow;

	void Awake(){
		objToFollow = GameObject.FindGameObjectWithTag ("Player");
		if (Vector2.Distance (transform.position, objToFollow.transform.position) > 2) {
			transform.position = new Vector3(objToFollow.transform.position.x,objToFollow.transform.position.y,-10);
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (objToFollow == null) { return; }
		transform.position = Vector3.Lerp (transform.position,
			new Vector3 (objToFollow.transform.position.x, objToFollow.transform.position.y, -10),10*Time.deltaTime);
	}
}
