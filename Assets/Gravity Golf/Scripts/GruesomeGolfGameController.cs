using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GruesomeGolfGameController : MonoBehaviour {
    public static float gravityConstant = -9.81f;

    private void Awake() {
        Physics.gravity = new Vector3(0, 0, gravityConstant);
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToPortrait = false;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    // Update is called once per frame
    void Update() {

    }
}
