using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookController : MonoBehaviour {
    public GameObject hook;
    LineRenderer lr;
    public float duration;
    public GameObject player;
    public delegate void OnContactAction();
    public static event OnContactAction OnContact;

    private void Start() {
        lr = GetComponent<LineRenderer>();
    }

    private void Update() {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, player.transform.position);
    }

    IEnumerator Timer() {
        yield return new WaitForSeconds(duration);
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        hook.SetActive(false);
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.tag != player.tag) {
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            hook.SetActive(true);
            StartCoroutine(Timer());
            if (OnContact != null) {
                OnContact();
            }
        }
    }

    private void OnDestroy() {
        OnContact = null;
    }
}
