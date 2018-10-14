using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float trajectoryWeight, touchWeight = 3, maxTime = 3, shotTolerance = 0.5f;
    Rigidbody rb;
    public Rigidbody recieverRB;
    public GameObject grapplingHook;
    public CameraController cam;
    public LineRenderer touchInput, shotProjection;
    private int size = 25;
    public static GameObject player;

    public GameObject bullet;
    public ParticleSystem deathPS;

    Vector3 startPosition = Vector3.zero;
    GameManager manager;

    // Use this for initialization

    public enum FireMode {
        regular, bulletTime, grapplingHook, disabled
    }

    public FireMode fireMode = FireMode.regular;

    private void Awake() {
        player = gameObject;
    }

    void Start () {
        manager = GameManager.instance;
        GameManager.OnGameOver += OnDeath;
        GameManager.OnReset += Reset;
        rb = GetComponent<Rigidbody>();
        shotProjection.positionCount = size;
	}

    private void Reset() {
        gameObject.SetActive(true);
    }

    void OnDeath() {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    /*
     * Kinematic equations: 
     *  T = (V_f - V_i)/A
     *  D = (V_i * T) + (0.5 (A * T^2))
     *  D = ((V_i + V_f)/2) * T
     */
    Vector3 velocity, shotVelocity;
    Vector3 realStartPos = Vector3.zero, realEndPos = Vector3.zero;
    void FixedUpdate () {
        /*
        if(Input.GetAxis("Fire1") == 1) {
            lr1.enabled = true;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
            velocity = (transform.position - mousePos) * trajectoryWeight;
            velocity = new Vector3(velocity.x, 0f, velocity.z);
            float x, z, timeTemp;
            for (int i = 0; i < size; i++) {
                timeTemp = -Mathf.Abs(maxTime * i / size);
                x = velocity.x * timeTemp;
                z = (velocity.z * timeTemp) + (Physics.gravity.z * (timeTemp * timeTemp) / 2);
                lr1.SetPosition(i, transform.position + new Vector3(x, 0f, z));
            }
        }
        if (Input.GetButtonUp("Fire1")) {
            lr1.enabled = false;
            cam.player = bullet.gameObject;
            bullet.gameObject.SetActive(true);
            bullet.GetComponent<Rigidbody>().velocity = -velocity;
            
        }
        */
        Vector3 pos;
        if (Input.touchCount > 0) {
            Touch t = Input.GetTouch(0);
            TouchPhase phase = t.phase;
            if (phase == TouchPhase.Began) {
                realStartPos = Camera.main.ScreenToViewportPoint(t.position);
                realStartPos = new Vector3(realStartPos.x, 0, realStartPos.y);
            } else if (phase == TouchPhase.Moved || phase == TouchPhase.Stationary) {
                realEndPos = Camera.main.ScreenToViewportPoint(t.position);
                realEndPos = new Vector3(realEndPos.x, 0, realEndPos.y);
                pos = transform.position + (realStartPos - realEndPos) * touchWeight;
                PrepareShot(pos);
                //ShowTouchInput(realStartPos, realEndPos);
            } else if (phase == TouchPhase.Ended) {
                if (( shotVelocity ).sqrMagnitude > shotTolerance) {
                    fireShot();
                } else {
                    shotProjection.enabled = false;
                    touchInput.enabled = false;
                }
            }
        }

        if(Input.GetAxis("Fire2") == 1) {
            PrepareShot(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y)));
        }

        if (Input.GetButtonUp("Fire2")) {
            fireShot();
        }
    }

    void PrepareShot(Vector3 pos) {
        if (fireMode != FireMode.disabled) {
            shotProjection.enabled = true;
            Vector3 mousePos = pos;
            shotVelocity = ( transform.position - mousePos ) * trajectoryWeight;
            shotVelocity = new Vector3(shotVelocity.x, 0f, shotVelocity.z);
            float x, z, timeTemp;
            for (int i = 0; i < size; i++) {
                timeTemp = -Mathf.Abs(maxTime * i / size);
                x = shotVelocity.x * timeTemp;
                z = ( shotVelocity.z * timeTemp ) + ( Physics.gravity.z * ( timeTemp * timeTemp ) / 2 );
                shotProjection.SetPosition(i, transform.position + new Vector3(x, 0f, z));
            }
        }
    }

    void ShowTouchInput(Vector3 start, Vector3 end) {
        touchInput.enabled = true;
        touchInput.SetPosition(0, end);
        touchInput.SetPosition(1, start);
    }

    void fireShot() {
        shotProjection.enabled = false;
        touchInput.enabled = false;

        switch (fireMode) {
            case FireMode.regular:
                recieverRB.velocity = -shotVelocity;
                rb.velocity = -shotVelocity;
                break;
            case FireMode.bulletTime:
                bullet.SetActive(true);
                bullet.transform.SetParent(null, true);
                bullet.transform.position = transform.position;
                bullet.GetComponent<Rigidbody>().velocity = -shotVelocity;
                fireMode = FireMode.regular;
                break;
            case FireMode.grapplingHook:
                grapplingHook.SetActive(true);
                grapplingHook.transform.SetParent(null, true);
                grapplingHook.transform.position = transform.position;
                grapplingHook.GetComponent<Rigidbody>().velocity = -shotVelocity;
                fireMode = FireMode.regular;
                break;
        }
    }
}
