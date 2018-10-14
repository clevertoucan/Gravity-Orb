using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
    public float transitionTime = 1f, spacing = .5f, fadeTime = .5f;
    int currentLevelScore = 0;
    public Text levelComplete, closeCall, timeBonus, ace, enemiesDestroyed, noAbilities, totalLabel;
    public Text levelCompleteScore, closeCallScore, timeBonusScore, aceScore, enemiesDestroyedScore, noAbilitiesScore, totalScore;
    public Image blurScreen;
    MapGenerator mapGenerator;
    public static ScoreController instance;
    public MaskableGraphic[] uiElements;
    public Material playerMaterial;
    Score score;
    GameManager manager;

    // Use this for initialization
    private void Awake() {
        instance = this;
    }

    void Start () {
        manager = GameManager.instance;
        score = Score.instance;
        mapGenerator = MapGenerator.instance;
        currentLevelScore = Persistance.instance.ReadData("score", 0);
        GameManager.OnNextLevel += NextLevel;
	}

    public void NextLevel() {
        blurScreen.material.SetFloat("_Size", 0);
        totalScore.color = Color.clear;
        totalLabel.color = Color.clear;
        currentLevelScore = score.TotalScore ;
        totalScore.text = currentLevelScore.ToString();
        mapGenerator.GenerateMap();
        foreach (MaskableGraphic i in uiElements) {
            i.gameObject.SetActive(false);
        }
    }

    public IEnumerator DisplayScore() {
        totalScore.color = Color.white;
        totalLabel.color = Color.white;
        totalScore.text = currentLevelScore.ToString();
        if (score.LevelCompleteScore > 0) {
            StartCoroutine(DisplayScore(levelComplete, levelCompleteScore, score.LevelCompleteScore));
            yield return new WaitForSecondsRealtime(spacing);
        }
        if (score.CloseCallScore > 0) {
            StartCoroutine(DisplayScore(closeCall, closeCallScore, score.CloseCallScore));
            yield return new WaitForSecondsRealtime(spacing);
        }
        if (score.EnemiesDestroyedScore > 0) {
            StartCoroutine(DisplayScore(enemiesDestroyed, enemiesDestroyedScore, score.EnemiesDestroyedScore));
            yield return new WaitForSecondsRealtime(spacing);
        }
        if (score.NoAbilitiesScore > 0) {
            StartCoroutine(DisplayScore(noAbilities, noAbilitiesScore, score.NoAbilitiesScore));
            yield return new WaitForSecondsRealtime(spacing);
        }
        if (score.AceScore > 0) {
            StartCoroutine(DisplayScore(ace, aceScore, score.AceScore));
            yield return new WaitForSecondsRealtime(spacing);
        }
        if (score.TimeScore > 0) {
            StartCoroutine(DisplayScore(timeBonus, timeBonusScore, score.TimeScore));
            yield return new WaitForSecondsRealtime(spacing);
        }
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn() {
        float startTime = Time.time;
        foreach (MaskableGraphic i in uiElements) {
            i.gameObject.SetActive(true);
        }
        while (Time.time - startTime < fadeTime) {
            float percentage = ( Time.time - startTime ) / fadeTime;
            foreach (MaskableGraphic i in uiElements) {
                i.color = new Color(i.color.r, i.color.g, i.color.b, percentage);
            }
            yield return null;
        }
    }

    IEnumerator DisplayScore(Text label, Text scoreText, int value) {
        float startTime = Time.time, percentage = 0f;
        Vector2 origin = label.transform.position;
        Vector2 start = origin;
        Vector2 target = new Vector2(start.x, start.y - 50);
        scoreText.text = value.ToString();
        while (Time.time - startTime < transitionTime / 2) {
            percentage = ( Time.time - startTime ) / ( transitionTime / 4 );
            label.color = Color.Lerp(Color.clear, Color.white, percentage * 4);
            scoreText.color = label.color;
            label.transform.position = Vector2.Lerp(start, target, percentage);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(.5f);
        startTime = Time.time;
        start = label.transform.position;
        target = totalScore.transform.position;
        target = new Vector2(start.x, target.y);
        int startScore = currentLevelScore;
        while (Time.time - startTime < transitionTime / 2) {
            percentage = ( Time.time - startTime ) / ( transitionTime / 4 );
            label.color = Color.Lerp(Color.white, Color.clear, percentage);
            scoreText.color = label.color;
            currentLevelScore = (int) Mathf.Lerp(startScore, startScore + value, percentage);
            totalScore.text = currentLevelScore.ToString();
            label.transform.position = Vector2.Lerp(start, target, percentage);
            yield return null;
        }
        currentLevelScore = startScore + value;
        totalScore.text = currentLevelScore.ToString();
        label.transform.position = origin;
    }

    private void OnDestroy() {
        blurScreen.material.SetFloat("_Size", 0);
    }
}
