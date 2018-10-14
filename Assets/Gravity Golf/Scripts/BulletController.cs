using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public GameObject explosionParticleSystem;

    private void OnTriggerEnter(Collider other) {
        if (other.transform.tag != "Player") {
            
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);
            explosionParticleSystem.SetActive(false);
            explosionParticleSystem.SetActive(true);
            explosionParticleSystem.transform.position = transform.position;
            var main = explosionParticleSystem.GetComponent<ParticleSystem>().main;
            transform.localPosition = Vector3.zero;
            if (other.tag == "EnemyBase") {
                EnemyController e = other.gameObject.GetComponentInChildren<EnemyController>();
                if (e != null) {
                    e.Destroy();
                }
            }
        }
    }
}
