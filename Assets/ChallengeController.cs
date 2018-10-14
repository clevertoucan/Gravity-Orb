using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeController : MonoBehaviour {
    public bool generateMap;
    public GameObject[] spawn;
    public int startSpawn, spawnRateOverTime;
    public enum VictoryCondition {
        EnemiesDestroyed, GoalReached
    }
    public enum DefeatCondition {
        PlayerDeath, TimeExpiration
    }
    public VictoryCondition[] victoryConditions;
    public DefeatCondition[] defeatConditions;
    
}
