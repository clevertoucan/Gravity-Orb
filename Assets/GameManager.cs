using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance; 

    public bool godMode;
    public GameObject player, goal;

    public delegate void VoidAction();
    public static event VoidAction OnGameOver, OnWin, OnNextLevel, OnReset;

    private void Awake() {
        instance = this;
    }


    // Update is called once per frame
    void Update () {
		
	}

    public void NextLevel() {
        if (OnNextLevel != null) {
            OnNextLevel();
        }
    }

    public void Win() {
        if (OnWin != null) {
            OnWin();
        }
    }

    public void GameOver() {
        if (!godMode) {
            if (OnGameOver != null) {
                OnGameOver();
            }
        }
    }

    public void Reset() {
        if (OnReset != null) {
            OnReset();
        }
    }

    private void OnDestroy() {
        OnGameOver = null;
        OnNextLevel = null;
        OnWin = null;
        OnReset = null;
    }
}
