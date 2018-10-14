using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : MonoBehaviour {
    public ReboundScattershotController parent;

    private void OnCollisionEnter(Collision collision) {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        parent.Contact(collision);
    }
}
