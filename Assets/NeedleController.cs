using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleController : MonoBehaviour {
    public ParticleSystem collisionPS;
    Vector3 lastPos;
	// Use this for initialization
	void Start () {
        lastPos = transform.position;
	}

    // Update is called once per frame
    Vector3 pos;
    void Update () {
        pos = Input.mousePosition;
        if ( Input.touchSupported && Input.touchCount > 0) {
            Touch t = Input.GetTouch(0);
            pos = t.position;
        }
        pos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.transform.position.y + 2.5f));
        transform.position = pos;
        transform.LookAt(transform.position + ( pos - lastPos ));
        lastPos = pos;
    }

    private void OnTriggerEnter(Collider other) {
        collisionPS.Play();
        EnemyController e = other.gameObject.GetComponent<EnemyController>();
        if (e != null) {
            e.Destroy();
        }
    }

    private void OnTriggerExit(Collider other) {
        collisionPS.Play();
    }


}
