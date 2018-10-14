using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour {
    public GameObject SandTemplate;
    GameObject sand;
    public int x;

    public Vector2 Orientation {
        get {
            return axes;
        }
    }
    Vector2 axes = Vector2.zero;

    private void Awake() {
        for (int i = 0; i < x; i++) {
            float y = Random.Range(1f, 9f);
            sand = Instantiate(SandTemplate, transform);
            sand.transform.localPosition = new Vector3(Random.Range(-y / 2.5f, y / 2.5f), y);
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}