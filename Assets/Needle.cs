using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Needle : AbilityWithCooldown {
    public GameObject needle;
    PlayerController player;
    [Range(0, 1)]
    public float timeScale = 0.05f, fixedTimeStep = .00001f;
    public float regularFOV, needleFOV, zoomDuration;
    float regularFixedTimeStep;
    public CinemachineVirtualCamera cam;
    protected override void Start() {
        base.Start();
        player = GetComponent<PlayerController>();
        regularFixedTimeStep = Time.fixedDeltaTime;
    }

    public IEnumerator Zoom(float start, float end) {
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime - startTime < zoomDuration) {
            float percentage = Mathf.Sin(Mathf.Sqrt((Mathf.PI / 2) * ( Time.unscaledTime - startTime ) / zoomDuration));
            cam.m_Lens.FieldOfView = Mathf.Lerp(start, end, percentage);
            yield return null;
        }
    }

    public override void StartAbility() {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = fixedTimeStep;
        player.fireMode = PlayerController.FireMode.disabled;
        needle.SetActive(true);
        StartCoroutine(Zoom(regularFOV, needleFOV));
    }

    public override void StopAbility() {
        Time.timeScale = 1;
        player.fireMode = PlayerController.FireMode.regular;
        needle.SetActive(false);
        Time.fixedDeltaTime = regularFixedTimeStep;
        StartCoroutine(Zoom(needleFOV, regularFOV));
    }
}
