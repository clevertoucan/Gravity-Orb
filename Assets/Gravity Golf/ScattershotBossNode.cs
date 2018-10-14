using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotBossNode : MonoBehaviour {
    public EnemyController parent;
    public float startScale = 1, endScale = 0.2f, distance = 1, fireForce = 10, adjustment = 0.35f;
    float cooldown;
    Material playerMaterial, defaultEnemyMaterial;
    [Range(0, 2)]
    public float angleCoverage = 0.9f;
    public bool isFiring;
    List<GameObject> bullets = new List<GameObject>();

    private void Start() {
        foreach (Transform child in transform) {
            bullets.Add(child.gameObject);
        }
        cooldown = parent.cooldown;
        playerMaterial = parent.playerMaterial;
        defaultEnemyMaterial = parent.defaultEnemyMaterial;
    }

    public IEnumerator FireSequence() {
        isFiring = true;
        Reset();
        float startTime = Time.time;
        while (Time.time < startTime + cooldown / 2) {
            bool alternator = true;
            for (int i = 0; i < bullets.Count; i++) {
                float progress = ( Time.time - startTime ) / ( cooldown / 2 );
                float currentScale = Mathf.Lerp(startScale, endScale, progress);
                float pos = (float)i / bullets.Count;
                bullets[i].transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                float angle = pos * Mathf.PI * angleCoverage + adjustment + ( Mathf.PI * ( 1 - progress ) * ( alternator ? 1 : -1 ) );
                bullets[i].transform.localPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * progress * distance;
                bullets[i].GetComponent<MeshRenderer>().material.color = Color.Lerp(defaultEnemyMaterial.color, playerMaterial.color, progress);
                alternator = !alternator;
            }
            yield return null;
        }
        Fire();
        isFiring = false;
    }

    void Fire() {
        foreach (GameObject b in bullets) {
            b.transform.SetParent(null, true);
            b.GetComponent<Collider>().enabled = true;
            b.GetComponent<Rigidbody>().isKinematic = false;
            b.GetComponent<Rigidbody>().velocity = ( b.transform.position - transform.position ) * fireForce;
            
        }
    }

    private void Reset() {
        foreach (GameObject b in bullets) {
            b.transform.SetParent(transform, true);
            b.GetComponent<Collider>().enabled = false;
            b.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
