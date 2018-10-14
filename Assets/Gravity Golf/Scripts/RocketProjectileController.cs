using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectileController : EnemyController {
    Rigidbody rb;
    public GameObject rocketTip;
    public ParticleSystem explosionPS, trail;
    public float speedMultiplier = 10f, explosionRadius = 25f, selfDestructTimer = 5f, distance;
    float startTime;
    Vector3 startPosition;
    Quaternion startRotation;
    public GameObject parent;
    public bool bossRocket;
    bool fireSequenceStarted = false;

	// Use this for initialization
	new protected void Start () {
        base.Start();
        player = GameManager.instance.player;
        rb = GetComponent<Rigidbody>();
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
        if (bossRocket) {
            cooldown = parent.GetComponent<EnemyController>().cooldown / 2;
        } else {
            StartCoroutine(EnemyAILoop());
        }
    }

    private void Update() {
        if (transform.parent != null && !bossRocket) {
            transform.parent.LookAt(player.transform);
        }
    }

    public override void Delete() {
        Destroy(explosionPS.gameObject);
        Destroy(parent);
        Destroy(gameObject);
    }
	
	// Update is called once per frame

    public override IEnumerator FireSequence() {
        fireSequenceStarted = true;
        float startTime = Time.time;
        float onTime = Time.time, offTime = Time.time;
        float percentage = 0f;
        while (Time.time - startTime < cooldown) {
            percentage = ( Time.time - startTime ) / ( cooldown );
            float delta = Mathf.Lerp(0, distance, percentage);
            rocketTip.transform.localPosition = new Vector3(0, 0, delta);
            Color c = Color.Lerp(Color.white, playerMaterial.color, percentage);
            GetComponent<MeshRenderer>().material.color = c;
            rocketTip.GetComponent<MeshRenderer>().material.color = c;
            if (!bossRocket) {
                transform.LookAt(player.transform);
            }
            yield return null;
        }
        StartCoroutine(Fire());
    }

    void Explode() {
        if (parent == null) {
            Destroy(gameObject);
            return;
        }
        if (( player.transform.position - rocketTip.transform.position ).sqrMagnitude < explosionRadius) {
            GameManager.instance.GameOver();
        }
        isFiring = false;
        explosionPS.transform.SetParent(null, true);
        explosionPS.transform.position = rocketTip.transform.position;
        var psMain = explosionPS.main;
        psMain.startColor = playerMaterial.color;
        explosionPS.Play();
        transform.SetParent(parent.transform);
        transform.localPosition = startPosition;
        transform.localRotation = startRotation;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        GetComponent<MeshRenderer>().material.color = Color.white;
        rocketTip.GetComponent<MeshRenderer>().material.color = Color.white;
        rocketTip.transform.localPosition = Vector3.zero;
        trail.Stop();
    }

    private void OnTriggerEnter(Collider other) {
        if (isFiring) {
            Explode();
        }
    }

    IEnumerator Fire() {
        transform.SetParent(null, true);
        rb.isKinematic = false;
        startTime = Time.time;
        isFiring = true;
        fireSequenceStarted = false;
        trail.Play();
        Vector3 target;
        float startAngle = Mathf.Acos(transform.forward.x);
        if (transform.forward.z < 0) {
            startAngle = -startAngle;
        }
        while (Time.time - startTime < selfDestructTimer && isFiring) {
            rb.AddForce(transform.forward * speedMultiplier);
            if (scrambled) {
                float angle = ( Time.time - startTime ) * Mathf.PI + startAngle;
                target = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            } else {
                target = player.transform.position;
            }
            transform.LookAt(target);
            var main = trail.main;
            main.startColor = playerMaterial.color;
            yield return null;
        }
        if (isFiring) {
            Explode();
        }
    }

    public bool IsFiring {
        get {
            return isFiring || fireSequenceStarted;
        }
    }

}
