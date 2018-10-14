using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorReciever : MonoBehaviour {
    public MaskableGraphic[] baseBackground, background, foreground;
    ColorController c;
    public bool complementary;
    
    private void Start() {
        ColorController.OnColorSet += SetColor;
        c = ColorController.instance;
    }

    void SetColor(Color themeColor, Color backgroundColor, Color foregroundColor, Color accentColor) {
        Color col = complementary ? ColorRYB.Complementary(foregroundColor) : foregroundColor;
        foreach (MaskableGraphic g in baseBackground) {
            g.color = col;
        }
        col = complementary ? ColorRYB.Complementary(backgroundColor) : backgroundColor;
        foreach (MaskableGraphic g in background) {
            g.color = col;
        }
        col = complementary ? ColorRYB.Complementary(accentColor) : accentColor;
        foreach (MaskableGraphic g in foreground) {
            g.color = col;
        }
    }
}
