using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundController : MonoBehaviour {


    private void OnTriggerEnter2D(Collider2D collision) {
        collision.transform.position *= -1;
    }
}
