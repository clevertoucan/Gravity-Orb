using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBeacon : MonoBehaviour {
    static GameObject target;

    public static GameObject Target {
        get {
            return target;
        }
    }

    private void Awake() {
        target = gameObject;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
