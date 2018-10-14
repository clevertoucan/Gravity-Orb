using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class Recall : AbilityWithCooldown {
    Queue<Vector3> positions = new Queue<Vector3>();
    public float recallTime = 3f;
    public PostProcessingProfile defaultProfile, wonky;
    float runtime, mapGenTime;

    protected override void Start() {
        base.Start();
        GameManager.OnNextLevel += ResetPositionData;
    }

    public void ResetPositionData() {
        positions.Clear();
    }

    private void FixedUpdate() {
        positions.Enqueue(transform.position);
        if (positions.Count > recallTime / Time.fixedUnscaledDeltaTime) {
            positions.Dequeue();
        }
    }

    IEnumerator BackToThePast() {
        float startTime = Time.unscaledTime;
        List<Vector3> positionList = new List<Vector3>(positions);
        Camera.main.GetComponent<PostProcessingBehaviour>().profile = wonky;
        GetComponent<Rigidbody>().isKinematic = true;
        MapGenerator.instance.godMode = true;
        while (Time.unscaledTime - startTime < effectDuration) {
            float percentage = ( Time.unscaledTime - startTime ) / effectDuration;
            Time.timeScale = percentage;
            int index = (int) (percentage * percentage * positionList.Count);
            transform.position = positionList[(positionList.Count - 1) - index];
            yield return null;
        }
        MapGenerator.instance.godMode = false;
        Time.timeScale = 1;
        Camera.main.GetComponent<PostProcessingBehaviour>().profile = defaultProfile;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public override void StartAbility() {
        StartCoroutine(BackToThePast());
    }

    public override void StopAbility() {
    }
}
