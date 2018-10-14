using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorWheel : MonoBehaviour {
    LineRenderer lr;
    public bool ryb;
	// Use this for initialization
	void Start () {

        SetLineRenderer();
	}

    public void SetLineRenderer() {
        lr = GetComponent<LineRenderer>();
        GradientColorKey[] keys = new GradientColorKey[8];
        int x = 0;
        for (int i = 0; i < lr.positionCount; i++) {
            lr.SetPosition(i, new Vector3(Mathf.Sin(( (float)i / lr.positionCount ) * Mathf.PI * 2), Mathf.Cos(( (float)i / lr.positionCount ) * Mathf.PI * 2)));
            if ((i) % ( lr.positionCount / 7 ) == 0) {
                if (ryb) {
                    keys[x].color = ColorRYB.RYBtoRGB(ColorRYB.HSVtoRYB(new ColorHSV((float)i / lr.positionCount, 1, 1)));
                } else {
                    keys[x].color = Color.HSVToRGB((float)i / lr.positionCount, 1, 1);
                }
                keys[x].time = (float)i / lr.positionCount;
                x++;
            }
        }
        GradientAlphaKey[] alpha = new GradientAlphaKey[2];
        alpha[0].alpha = 1;
        alpha[0].time = 0;
        alpha[1].time = 1;
        alpha[1].alpha = 1;
        Gradient g = new Gradient();
        g.colorKeys = keys;
        lr.colorGradient = g;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
