using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    Persistance persistance;
    public float maxTime, winBonus = 20f, lerpTime = 2f;
    public static TimeManager instance;
    public delegate void TimeBankUpdatedAction(float currentTime);
    public static event TimeBankUpdatedAction OnTimeBankUpdated;
    bool paused = false;
    float startTime;
    GameManager manager;

    private void Awake() {
        instance = this;
    }

    public float TimeBank {
        private set;
        get;
    }

    public float DeltaLevelTime {
        get {
            Debug.Log(TimeBank - startTime);
            return TimeBank - startTime;
        }
    }
    
	// Use this for initialization
	void Start () {
        manager = GameManager.instance;
        persistance = Persistance.instance;
        //TimeBank = persistance.ReadData("TimeManager.timeBank", 90f);
        //startTime = persistance.ReadData("TimeManager.startTime", 60f);
        TimeBank = 90f;
        startTime = 60f;
        GameManager.OnWin += AddTime;
        GameManager.OnWin += Pause;
        GameManager.OnGameOver += Pause;
        GameManager.OnReset += Unpause;
        GameManager.OnNextLevel += Unpause;
	}

    void AddTime() {
        startTime = TimeBank;
        StartCoroutine(LerpTime());

    }

    IEnumerator LerpTime() {
        float startTime = Time.unscaledTime;
        float startTimeBank = TimeBank;
        while (Time.unscaledTime - startTime < lerpTime) {
            float percentage = ( Time.unscaledTime - startTime ) / lerpTime;
            TimeBank = startTimeBank + percentage * winBonus;
            if (OnTimeBankUpdated != null) {
                OnTimeBankUpdated(TimeBank);
            }
            yield return null;
        }
    }

    private void Update() {
        if (!paused) {
            TimeBank -= Time.deltaTime;
            if (OnTimeBankUpdated != null) {
                OnTimeBankUpdated(TimeBank);
            }
        }
    }

    void Unpause() {
        paused = false;
    }

    void Pause() {
        paused = true;
    }

    private void OnDestroy() {
        persistance.WriteData("TimeManager.timeBank", TimeBank);
    }
}
