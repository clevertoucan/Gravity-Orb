using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReboundScattershotController : EnemyController {
    public float startScale = 1, endScale = .2f, distance = 1, fireForce = 10;
    public float adjustment = 0;
    [Range(0, 2)]
    public float angleCoverage;
    List<GameObject> bullets = new List<GameObject>();
    public GameObject container;
    // Use this for initialization
    new private void Start() {
        base.Start();
        StartCoroutine(EnemyAILoop());
        foreach (Transform child in transform) {
            if (child.GetComponent<ScattershotBulletController>() != null) {
                bullets.Add(child.gameObject);
            }
        }

    }

    // Update is called once per frame
    public override IEnumerator FireSequence() {
        isFiring = true;
        Reset();
        float startTime = Time.time;
        while (Time.time < startTime + cooldown / 4) {
            bool alternator = true;
            for (int i = 0; i < bullets.Count; i++) {
                float progress = ( Time.time - startTime ) / ( cooldown / 4 );
                float currentScale = Mathf.Lerp(startScale, endScale, progress);
                float pos = (float)i / bullets.Count;
                if (scrambled) {
                    pos = -pos;
                }
                bullets[i].transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                float angle = pos * Mathf.PI * angleCoverage + adjustment + ( Mathf.PI * ( 1 - progress ) * ( alternator ? 1 : -1 ) );
                bullets[i].transform.localPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * progress * distance;
                bullets[i].GetComponent<MeshRenderer>().material.color = Color.Lerp(defaultEnemyMaterial.color, playerMaterial.color, progress);
                transform.LookAt(player.transform);
                alternator = !alternator;
            }
            yield return null;
        }
        startTime = Time.time;
        while (Time.time < startTime + cooldown / 4) {
            bool alternator = true;
            for (int i = 0; i < bullets.Count; i++) {
                float progress = ( Time.time - startTime ) / ( cooldown / 4 );
                float pos = Mathf.Lerp((float)i / bullets.Count, 1f / 2f, progress);
                if (scrambled) {
                    pos = -pos;
                }
                float angle = pos * Mathf.PI * angleCoverage + adjustment * ( alternator ? 1 : -1 );
                bullets[i].transform.localPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
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
            b.SetActive(false);
        }
        container.SetActive(true);
        container.GetComponent<Collider>().enabled = true;
        container.GetComponent<MeshRenderer>().enabled = true;
        Vector3 diff = ( player.transform.position - transform.position ).normalized;
        if (scrambled) {
            diff = -diff;
        }
        container.transform.position = transform.position + diff * distance;
        container.transform.localScale = new Vector3(endScale, endScale, endScale);
        container.GetComponent<Rigidbody>().isKinematic = false;
        container.GetComponent<Rigidbody>().velocity = (container.transform.position - transform.position ) * fireForce;
        container.GetComponent<MeshRenderer>().material = playerMaterial;
    }

    private void Reset() {
        container.SetActive(false);
        foreach (GameObject b in bullets) {
            b.SetActive(true);
            b.transform.SetParent(transform);
            b.GetComponent<Collider>().enabled = false;
            b.GetComponent<Rigidbody>().isKinematic = true;
            b.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void Contact(Collision other) {
        bool alternator = true;
        Vector3 v = container.transform.position + (other.contacts[0].point - container.transform.position).normalized;
        container.transform.LookAt(v);
        for (int i = 1; i < bullets.Count; i++) {
            bullets[i].SetActive(true);
            bullets[i].transform.SetParent(container.transform, true);
            bullets[i].GetComponent<Collider>().enabled = true;
            bullets[i].GetComponent<Rigidbody>().isKinematic = false;
            float pos = (float)i / bullets.Count;
            float angle = pos * Mathf.PI * angleCoverage + adjustment + ( Mathf.PI * ( alternator ? 1 : -1 ) );
            bullets[i].transform.localPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) / 10;
            alternator = !alternator;
            bullets[i].GetComponent<Rigidbody>().velocity = ( bullets[i].transform.position - container.transform.position ).normalized * fireForce;
        }
    }
}
