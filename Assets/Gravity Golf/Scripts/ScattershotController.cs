using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotController : EnemyController {
    public float startScale, endScale, distance, fireForce;
    public float adjustment;
    [Range(0, 2)]
    public float angleCoverage;
    List<GameObject> bullets = new List<GameObject>();
    // Use this for initialization
    new private void Start() {
        base.Start();
        StartCoroutine(EnemyAILoop());
        foreach (Transform child in transform) {
            bullets.Add(child.gameObject);
        }
        
    }

    // Update is called once per frame
    public override IEnumerator FireSequence() {
        isFiring = true;
        Reset();
        float startTime = Time.time;
        while (Time.time < startTime + cooldown / 2) {
            bool alternator = true;
            for (int i = 0; i < bullets.Count; i++) {
                float progress = ( Time.time - startTime ) / ( cooldown / 2 );
                float currentScale = Mathf.Lerp(startScale, endScale, progress);
                float pos = (float) i / bullets.Count;
                if (scrambled) {
                    pos = -pos;
                }
                bullets[i].transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                float angle = pos * Mathf.PI * angleCoverage + adjustment + (Mathf.PI * (1 - progress) * (alternator? 1 : -1));
                bullets[i].transform.localPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * progress * distance;
                bullets[i].GetComponent<MeshRenderer>().material.color = Color.Lerp(defaultEnemyMaterial.color, playerMaterial.color, progress);
                transform.LookAt(player.transform);
                alternator = !alternator;
            }
            yield return null;
        }
        Fire();
        isFiring = false;
    }

    void Fire() {
        foreach (GameObject b in bullets) {
            b.GetComponent<Collider>().enabled = true;
            b.GetComponent<Rigidbody>().isKinematic = false;
            b.GetComponent<Rigidbody>().velocity = ( b.transform.position - transform.position ) * fireForce;
        }
    }

    private void Reset() {
        foreach (GameObject b in bullets) {
            b.GetComponent<Collider>().enabled = false;
            b.GetComponent<Rigidbody>().isKinematic = true;
            b.GetComponent<MeshRenderer>().enabled = true;
        }
    }

}
