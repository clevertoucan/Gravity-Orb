using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesScoreController : MonoBehaviour {
    public Text score;

	// Use this for initialization
	void Awake () {
        Score.OnScoreInit += ShowScore;
        Score.OnScoreChange += ShowScore;
	}

    void ShowScore() {
        score.text = Score.instance.TotalScore.ToString();
    }
}
