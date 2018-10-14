using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GravityGolfGameController : MonoBehaviour {
    public static float gravityConstant = 9.81f;

    public Vector3 Orientation {
        get {
            return axes;
        }
    }

    Vector3 axes = Vector3.zero;

    private void Awake() {
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToPortrait = false;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    // Update is called once per frame
    void Update() {
        axes = Vector3.zero;
        float value = 0f;
        if (Application.platform == RuntimePlatform.Android) {
            if (Input.acceleration.sqrMagnitude > .1) {
                value = gravityConstant / Input.acceleration.magnitude;
                axes = Input.acceleration.normalized;
            }
        } else {
            axes = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if (axes.sqrMagnitude > .1) {
                value = gravityConstant / axes.magnitude;
            }
        }
        Physics.gravity = axes * value;
    }
}
