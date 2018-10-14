using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    public MaskableGraphic image;
    public RectTransform parentCanvas;
    ColorController colorController;
    TimeManager timeManager;
    float currentTime;
    // Use this for initialization
    void Start() {
        ColorController.OnColorSet += SetColor;
        colorController = ColorController.instance;
        timeManager = TimeManager.instance;
        currentTime = 90f;
    }

    void SetColor(Color themeColor, Color backgroundColor, Color foregroundColor, Color accentColor) {
        image.color = ColorRYB.Complementary(accentColor);
    }

    // Update is called once per frame
    float percentage;
    void Update() {
        percentage = timeManager.TimeBank / timeManager.maxTime;
        float x = percentage * ( parentCanvas.rect.width);
        image.rectTransform.sizeDelta = new Vector2(x, image.rectTransform.sizeDelta.y);
	}

    
}
