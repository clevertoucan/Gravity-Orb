using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 acc = Input.acceleration;
        transform.rotation = Quaternion.Euler(new Vector3(acc.x, acc.z, acc.y) * 360);
	}
}
