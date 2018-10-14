using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockController : MonoBehaviour {

    Text clock;
    public int startingMinutes = 1;
    int minutes;
    float seconds;
    int displaySeconds;

	// Use this for initialization
	void Start () {
        minutes = startingMinutes;
        seconds = minutes * 60;
        clock = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (minutes >= 0) {
            if (seconds - (minutes * 60) < 0) {
                minutes--;
                if (minutes != startingMinutes - 1) {
                    Physics2D.gravity *= -1;
                }
            }
            displaySeconds = (int)(seconds - (minutes * 60));
            clock.text = string.Format("{0:D2}:{1:D2}", minutes, displaySeconds);
            seconds -= Time.deltaTime;
        }
	}
}
