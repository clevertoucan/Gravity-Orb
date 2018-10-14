using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Restart : MonoBehaviour {
    public Image restartIcon;
    public float fadeTime = 2;
    GameManager manager;

    private void Start() {
        manager = GameManager.instance;
        GameManager.OnGameOver += ActiveOn;
        gameObject.SetActive(false);
    }

    void ActiveOn() {
        gameObject.SetActive(true);
    }

    private void OnEnable() {
        StartCoroutine(FadeIn());
    }

    public void ResetGame() {
        manager.Reset();
        restartIcon.color = new Color(restartIcon.color.r, restartIcon.color.g, restartIcon.color.b, 0f);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}

    IEnumerator FadeIn() {
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime < startTime + fadeTime) {
            float a = ( Time.unscaledTime - startTime ) / fadeTime;
            restartIcon.color = new Color(restartIcon.color.r, restartIcon.color.g, restartIcon.color.b, a);
            yield return null;
        }
    }
}
