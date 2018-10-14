using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotBulletController : MonoBehaviour {
    public GameObject ps;
    MeshGenerator mg = MeshGenerator.instance;

    private void OnTriggerEnter(Collider other) {
        if (other.tag != "EnemyBullet") {
            ps.GetComponent<ParticleSystem>().Play();
            Vector3 pos = transform.position;
            transform.localPosition = Vector3.zero;
            ps.transform.position = pos;
            //var main = ps.GetComponent<ParticleSystem>().main;
            //main.startColor = ColorController.instance.accentColor;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            if (other.transform.tag == "Player") {
                GameManager.instance.GameOver();
            }
        }
    }

}
