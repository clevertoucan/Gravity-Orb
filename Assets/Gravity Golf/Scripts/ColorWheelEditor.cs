using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof (ColorWheel))]
public class ColorWheelEditor : Editor {
    public override void OnInspectorGUI() {
        ColorWheel wheel = (ColorWheel)target;
        if (DrawDefaultInspector()) {
            wheel.SetLineRenderer();
        }
    }
}

#endif