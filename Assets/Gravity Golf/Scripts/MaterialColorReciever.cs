using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColorReciever : MonoBehaviour {
    public Material[] theme, foreground, background, accent;
    ColorController c;
    public bool complementary = false;

    private void Start() {
        ColorController.OnColorSet += SetColor;
        c = ColorController.instance;
    }

    void SetColor(Color themeColor, Color backgroundColor, Color foregroundColor, Color accentColor) {
        Color col = complementary ? ColorRYB.Complementary(foregroundColor) :foregroundColor;
        foreach (Material g in foreground) {
            g.color = col;
        }
        col = complementary ? ColorRYB.Complementary(backgroundColor) : backgroundColor;
        foreach (Material g in background) {
            g.color = col;
        }
        col = complementary ? ColorRYB.Complementary(accentColor) : accentColor;
        foreach (Material g in accent) {
            g.color = col;
        }
        col = complementary ? ColorRYB.Complementary(themeColor) : themeColor;
        foreach (Material g in theme) {
            g.color = col;
        }
    }
}
