using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperCube : MonoBehaviour {
    public float maxSpeed = 100, minTimeScale = .1f, maxTimeScale = 1f;
    Rigidbody rb;

    // Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

	}
}
