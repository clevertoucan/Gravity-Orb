using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private void Start() {
        ColorController.OnColorSet += SetColor;
    }

    void SetColor(Color themeColor, Color backgroundColor, Color foregroundColor, Color accentColor) {
        Camera.main.backgroundColor = backgroundColor;
    }

}
