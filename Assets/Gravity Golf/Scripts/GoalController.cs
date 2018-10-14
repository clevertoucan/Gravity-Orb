using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalController : MonoBehaviour {
    Rigidbody rb;
    public float goalSpeed = 10f, winSequenceTime = 3f, lastWin = 0;
    public GameObject player;
    public MeshGenerator mg;
    public MapGenerator mapGenerator;
    public Image blurScreen;
    public ParticleSystem ps;
    GameManager manager;
    bool won = false;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody>();
	}

    private void Start() {
        manager = GameManager.instance;
        GameManager.OnNextLevel += Enable;
        ps.Play();
    }

    void Enable() {
        gameObject.SetActive(true);
        won = false;
    }

    private void OnEnable() {
        transform.rotation = Random.rotation;
        rb.angularVelocity = new Vector3(0, 0, goalSpeed);
        ps.Play();
    }

    float gameTime = 0f;
    IEnumerator StartWinSequence() {
        manager.Win();
        gameTime = Time.time - gameTime;
        float startTime = Time.time;
        Vector3 startRotation = transform.rotation.eulerAngles;
        Vector3 playerRotation = player.transform.rotation.eulerAngles;
        Vector3 rotDifference = playerRotation - startRotation;
        Vector3 startPosition = transform.position;
        Vector3 playerPosition = player.transform.position;
        Vector3 posDifference = playerPosition - startPosition;
        Vector3 startScale = transform.localScale;
        Vector3 playerScale = player.transform.localScale;
        Vector3 scaleDifference = playerScale - startScale;
        float progress = 0;
        //rb.angularVelocity = Vector3.zero;
        player.GetComponent<Rigidbody>().isKinematic = true;
        won = true;
        foreach (EnemyController e in mapGenerator.spawnedEnemies) {
            if (e != null) {
                e.Delete();
            }
        }
        mapGenerator.spawnedEnemies.Clear();
        while (Time.time < startTime + winSequenceTime) {
            progress  = ( ( Time.time - startTime) / ( winSequenceTime ) );
            transform.rotation = Quaternion.Euler(startRotation + rotDifference * progress );
            transform.position = startPosition + posDifference * progress;
            transform.localScale = startScale + scaleDifference * progress;
            blurScreen.material.SetFloat("_Size", Mathf.Lerp(0, 5, progress));
            yield return null;
        }
        ScoreController.instance.StartCoroutine(ScoreController.instance.DisplayScore());
        player.GetComponent<Rigidbody>().isKinematic = false;
        transform.localScale = startScale;
        ps.Stop();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && Time.time - lastWin > 5f) {
            lastWin = Time.time;
            StartCoroutine(StartWinSequence());
        }
    }
}
