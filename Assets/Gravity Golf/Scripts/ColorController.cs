using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour {
    public bool inGame = false;
    [Range(0,1)]
    public float backgroundValue = .1f, accentValue = 1, baseBackgroundValue = .3f,  accentSaturation, backgroundSaturation, foregroundSaturation;
    float currentAccentS, currentBackgroundS, currentForegroundSaturation;
    private Color backgroundColor, foregroundColor, accentColor;
    public delegate void ColorSetAction(Color themeColor, Color backgroundColor, Color foregroundColor, Color accentColor);
    public static event ColorSetAction OnColorSet;
    public Color themeColor;
    public float cycleLength = 10;

    public float startH;
    protected float s = 1, v = 1;
    public static ColorController instance;
    Vector3 startPosition, goalPosition;
    float startMagnitude;
    GameManager manager;
    private void Awake() {
        MapGenerator.OnMapGenerated += ResetPositionData;
        instance = this;
    }
    
    protected virtual void Start() {
        Load();
        manager = GameManager.instance;
        Color.RGBToHSV(themeColor, out startH, out s, out v);
        currentAccentS = accentSaturation;
        currentBackgroundS = backgroundSaturation;
        currentForegroundSaturation = foregroundSaturation;
    }

    void ResetPositionData() {
        startPosition = GameManager.instance.player.transform.position;
        goalPosition = GameManager.instance.goal.transform.position;
        startMagnitude = ( goalPosition - startPosition ).sqrMagnitude;
    }

    protected float lastCycle;
    protected virtual void Update() {
        if (Time.time > lastCycle + cycleLength) {
            lastCycle = Time.time;
        }
        startH = ( Time.time - lastCycle ) / ( cycleLength );
        if (startH < 0 || startH > 1) {
            lastCycle = Time.time;
            startH = 0;
        }
        if (inGame) { 
            float percentage = 1 - ( goalPosition - manager.player.transform.position ).sqrMagnitude / startMagnitude;
            Mathf.Clamp01(percentage);
            currentAccentS = accentSaturation * percentage;
            currentBackgroundS = backgroundSaturation * percentage;
            currentForegroundSaturation = foregroundSaturation * percentage;
        }
        themeColor = Color.HSVToRGB(startH, s, v);
        backgroundColor = Color.HSVToRGB(startH, currentBackgroundS, backgroundValue);
        foregroundColor = Color.HSVToRGB(startH, currentForegroundSaturation, baseBackgroundValue);
        accentColor = Color.HSVToRGB(startH, currentAccentS, accentValue);
        if (OnColorSet != null) {
            OnColorSet(themeColor, backgroundColor, foregroundColor, accentColor);
        }
    }

    protected Color Complementary(Color c, float v) {
        float h, s, val;
        Color.RGBToHSV(ColorRYB.Complementary(c), out h, out s, out val);
        return Color.HSVToRGB(h, s, v);
    }

    protected Color Complementary(Color c, float v, float increment) {
        float h, s, val;
        Color.RGBToHSV(ColorRYB.Complementary(c), out h, out s, out val);
        if (h < .5) {
            increment = -increment;
        }
        if (h + increment > 1) {
            return Color.HSVToRGB(( h + increment ) - 1, s, v);
        } else if (h + increment < 0) {
            return Color.HSVToRGB(1 + ( h + increment ), s, v);
        } else {
            return Color.HSVToRGB(h + increment, s, v);
        }
    }

    private void OnDestroy() {
        OnColorSet = null;
    }

    private void Load() {
        float cycleProgress = Persistance.instance.ReadData("ColorController.colorCycle", 0f);
        lastCycle = Time.time - cycleProgress * cycleLength;
    }
}
