using UnityEngine;

public class Score: MonoBehaviour {
    public static Score instance;
    public int baseTimeScore = 10, baseCloseCallScore = 50, baseAceScore = 500, baseEnemyScore = 100, baseNoAbilitiesScore = 250, baseLevelCompleteScore = 500;
    int levelAttempts = 0;
    int levelCompleteScore, timeScore, closeCallScore, aceScore, enemiesDestroyedScore, noAbilitiesScore, totalScore;
    Persistance persistance;
    public delegate void VoidAction();
    public static event VoidAction OnScoreInit;
    public static event VoidAction OnScoreChange;
    GameManager manager;
    public int LevelCompleteScore {
        get { return levelCompleteScore; }
    }
    public int TimeScore {
        get {
            return timeScore;
        }
    }
    public int CloseCallScore {
        get {
            return closeCallScore;
        }
    }
    public int AceScore {
        get {
            return aceScore;
        }
    }
    public int EnemiesDestroyedScore{
        get {
            return enemiesDestroyedScore;
        }
    }
    public int NoAbilitiesScore {
        get { return noAbilitiesScore; }
    }
    public int TotalLevelScore {
        get {
            return levelCompleteScore + timeScore + closeCallScore + aceScore + enemiesDestroyedScore + noAbilitiesScore;
        }
    }
    public int TotalScore {
        get {
            return totalScore;
        }
    }

    private void Awake() {
        instance = this;
    }

    private void Start() {
        manager = GameManager.instance;
        GameManager.OnGameOver += Death;
        GameManager.OnNextLevel += ClearLevelScore;
        GameManager.OnWin += Win;
        EnemyController.OnEnemyDestroyed += EnemyDestroyed;
        levelCompleteScore = baseLevelCompleteScore;
        timeScore = 0;
        closeCallScore = 0;
        aceScore = baseAceScore;
        enemiesDestroyedScore = 0;
        noAbilitiesScore = baseNoAbilitiesScore;
        persistance = Persistance.instance;
        totalScore = persistance.ReadData("score", 0);
        if (OnScoreInit != null) {
            OnScoreInit();
        }
    }

    public void BuyAbility(int scoreCost) {
        totalScore -= scoreCost;
        persistance.WriteData("score", totalScore);
        if (OnScoreChange != null) {
            OnScoreChange();
        }
    }

    void EnemyDestroyed(EnemyController e) {
        enemiesDestroyedScore += baseEnemyScore;
    }

    void CloseCall() {
        closeCallScore += baseCloseCallScore;
    }

    void Death() {
        aceScore = 0;
        levelAttempts++;
    }

    void AbilityUse() {
        noAbilitiesScore = 0;
    }

    void ClearLevelScore() {
        timeScore = 0;
        closeCallScore = 0;
        aceScore = baseAceScore;
        enemiesDestroyedScore = 0;
        noAbilitiesScore = baseNoAbilitiesScore;
    }

    void Win() {
        float t = TimeManager.instance.DeltaLevelTime * baseTimeScore;
        timeScore = t > 0? (int) t : 0;
        totalScore += TotalLevelScore;
        persistance.WriteData("score", totalScore);
    }

    private void OnDestroy() {
        OnScoreInit = null;
        OnScoreChange = null;
    }
}
