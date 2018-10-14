using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSColorReciever : MonoBehaviour {
    public ParticleSystem[] baseBackground, background, foreground;
    ColorController c;
    public bool complementary = false; 

    private void Start() {
        ColorController.OnColorSet += SetColor;
        c = ColorController.instance;
    }

    void SetColor(Color themeColor, Color backgroundColor, Color foregroundColor, Color accentColor) {
        Color col = complementary? ColorRYB.Complementary(foregroundColor) : foregroundColor;
        foreach (ParticleSystem p in baseBackground) {
            var main = p.main;
            main.startColor = col;
        }
        col = complementary? ColorRYB.Complementary(backgroundColor) : backgroundColor;
        foreach (ParticleSystem p in background) {
            var main = p.main;
            main.startColor = col;
        }
        col = complementary? ColorRYB.Complementary(accentColor) : accentColor;
        foreach (ParticleSystem p in foreground) {
            var main = p.main;
            main.startColor = col;
        }
    }
}
