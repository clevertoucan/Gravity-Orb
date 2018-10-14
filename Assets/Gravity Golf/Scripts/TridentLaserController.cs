using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TridentLaserController : EnemyController {
    public float fireTime = 3f, laserAngle = 90f, mainWidth = .5f, sideWidths = .3f, mainGuideWidth = .08f, sideGuideWidths = .05f;
    public LineRenderer leftLaser, mainLaser, rightLaser;
    public GameObject leftContainer, rightContainer, left, main, right;
    public float trailTime = .75f, trailDecayTime = .5f;
    Vector3 target;
    float width, leftWidth, rightWidth;
    public float maxAngularVelocity = 150f, tolerance = 1, angularAcceleration = 300f;
    Queue<LineRenderer> guides = new Queue<LineRenderer>();
    public GameObject guideParent;

    // Use this for initialization
    new void Start() {
        base.Start();
        foreach (LineRenderer r in guideParent.GetComponentsInChildren<LineRenderer>(true)) {
            guides.Enqueue(r);
        }
        StartCoroutine(EnemyAILoop());
        target = player.transform.position;
        width = mainLaser.widthMultiplier;
        rightWidth = rightLaser.widthMultiplier;
        leftWidth = leftLaser.widthMultiplier;
        LaserOff();
    }

    float yRot = 0, initialAngularVelocity = 0, currentAngularVelocity = 0, currentAngularAcceleration = 0;
    Vector3 heading, playerHeading;
    float angle;
    protected void Update() {
        playerHeading = ( player.transform.position - transform.position ).normalized;
        heading = transform.forward;
        angle = Vector3.SignedAngle(heading, playerHeading, Vector3.up);
        if (angle > 0) {
            currentAngularAcceleration = angularAcceleration;
        } else {
            currentAngularAcceleration = -angularAcceleration;
        }

        if (Mathf.Abs(angle) < tolerance && !scrambled) {
            /*
            float relativeVelocity = (player.GetComponent<Rigidbody>().velocity * Time.deltaTime).magnitude / ( player.transform.position - transform.position ).magnitude;
            Debug.Log(relativeVelocity);
            */
            currentAngularAcceleration = 0;
            initialAngularVelocity = 0;
        }

        if (scrambled) {
            currentAngularVelocity = initialAngularVelocity + currentAngularAcceleration * Time.deltaTime * 2;
        } else {
            currentAngularVelocity = Mathf.Clamp(initialAngularVelocity + currentAngularAcceleration * Time.deltaTime, -maxAngularVelocity, maxAngularVelocity);
        }
        yRot = yRot + Time.deltaTime * ( currentAngularVelocity + initialAngularVelocity ) / 2;
        transform.rotation = Quaternion.Euler(new Vector3(0, yRot, 0));
        initialAngularVelocity = currentAngularVelocity;
        target = transform.position + transform.forward;
    }
    /*
        protected IEnumerator FillPositions() {
            while (Time.time < trailTime) {
                positions.Enqueue(player.transform.position);
                yield return null;
            }
            filled = true;
        }
        */
    public override IEnumerator FireSequence() {
        isFiring = true;
        float startTime = Time.time;
        float onTime = Time.time, offTime = Time.time;
        float percentage = 0f;
        while (Time.time - startTime < cooldown / 2) {
            percentage = ( Time.time - startTime ) / ( cooldown / 2 );
            float delta = Mathf.Lerp(0, laserAngle, percentage);
            leftContainer.transform.localRotation = Quaternion.Euler(new Vector3(0f, delta, 0f));
            rightContainer.transform.localRotation = Quaternion.Euler(new Vector3(0f, -delta, 0f));
            float modifier = Mathf.Lerp(.4f, .05f, percentage);
            if (Time.time > onTime + modifier) {
                GuideOn(percentage);
                if (Time.time > onTime + modifier) {
                    onTime = Time.time;
                }
            }
            yield return null;
        }
        LaserOn();
        isFiring = false;
        while (Time.time - startTime < cooldown) {
            percentage = 2 * ( Time.time - ( startTime + ( cooldown / 2 ) ) ) / ( cooldown );
            float delta = Mathf.Lerp(laserAngle, 0, percentage);
            leftContainer.transform.localRotation = Quaternion.Euler(new Vector3(0f, delta, 0f));
            rightContainer.transform.localRotation = Quaternion.Euler(new Vector3(0f, -delta, 0f));
            mainLaser.widthMultiplier = width * ( 1 - percentage );
            leftLaser.widthMultiplier = leftWidth * ( 1 - percentage );
            rightLaser.widthMultiplier = rightWidth * ( 1 - percentage );
            yield return null;
        }
        mainLaser.widthMultiplier = 0;
        leftLaser.widthMultiplier = 0;
        rightLaser.widthMultiplier = 0;

    }

    IEnumerator DrawLaser(Vector3 start, Vector3 end, float duration, float thickness, float percentage) {
        LineRenderer laser = guides.Dequeue();
        laser.enabled = true;
        laser.SetPosition(0, start);
        laser.SetPosition(1, end);
        laser.widthMultiplier = thickness;
        float startTime = Time.time;
        Color baseColor = Color.Lerp(Color.white, playerMaterial.color, percentage);
        while (Time.time - startTime < duration) {
            float t = ( Time.time - startTime ) / duration;
            /*
            Gradient g = new Gradient();
            g.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) }, 
                new GradientAlphaKey[] { new GradientAlphaKey(a, 0), new GradientAlphaKey(a, 1) });
            laser.colorGradient = g;
            */
            Color c = Color.Lerp(baseColor, new Color(baseColor.r, baseColor.g, baseColor.b, 0), t);
            laser.startColor = c;
            laser.endColor = c;
            laser.widthMultiplier = thickness * ( 1 - t );
            yield return null;
        }
        guides.Enqueue(laser);
        laser.enabled = false;
    }

    void GuideOn(float percentage) {
        RaycastHit hitInfo, leftHitInfo, rightHitInfo;
        Vector3 direction = ( target - main.transform.position ).normalized;
        bool hit = Physics.Raycast(main.transform.position, direction, out hitInfo, 1000f);
        if (hit) {
            StartCoroutine(DrawLaser(main.transform.position, hitInfo.point, trailDecayTime, mainGuideWidth, percentage));
        } else {
            StartCoroutine(DrawLaser(main.transform.position, target, trailDecayTime, mainGuideWidth, percentage));
        }
        bool leftHit = Physics.Raycast(left.transform.position, direction, out leftHitInfo, 1000f);
        if (leftHit) {
            StartCoroutine(DrawLaser(left.transform.position, leftHitInfo.point, trailDecayTime, sideGuideWidths, percentage));
        } else {
            StartCoroutine(DrawLaser(left.transform.position, target, trailDecayTime, sideGuideWidths, percentage));
        }
        bool rightHit = Physics.Raycast(right.transform.position, direction, out rightHitInfo, 1000f);
        if (rightHit) {
            StartCoroutine(DrawLaser(right.transform.position, rightHitInfo.point, trailDecayTime, sideGuideWidths, percentage));
        } else {
            StartCoroutine(DrawLaser(right.transform.position, target, trailDecayTime, sideGuideWidths, percentage));
        }
    }

    void LaserOn() {
        mainLaser.SetPosition(0, main.transform.position);
        leftLaser.SetPosition(0, left.transform.position);
        rightLaser.SetPosition(0, right.transform.position);
        mainLaser.widthMultiplier = width;
        rightLaser.widthMultiplier = rightWidth;
        leftLaser.widthMultiplier = leftWidth;
        RaycastHit hitInfo, leftHitInfo, rightHitInfo;
        Vector3 direction = ( target - main.transform.position ).normalized;
        bool hit = Physics.Raycast(main.transform.position, direction, out hitInfo, 1000f);
        if (hit) {
            mainLaser.SetPosition(1, hitInfo.point);
        } else {
            mainLaser.SetPosition(1, target);
        }
        bool leftHit = Physics.Raycast(left.transform.position, direction, out leftHitInfo, 1000f);
        if (leftHit) {
            leftLaser.SetPosition(1, leftHitInfo.point);
        } else {
            leftLaser.SetPosition(1, target);
        }
        bool rightHit = Physics.Raycast(right.transform.position, direction, out rightHitInfo, 1000f);
        if (rightHit) {
            rightLaser.SetPosition(1, rightHitInfo.point);
        } else {
            rightLaser.SetPosition(1, target);
        }
        if (rightHitInfo.transform.tag == "Player"
            || leftHitInfo.transform.tag == "Player"
            || hitInfo.transform.tag == "Player") {

            GameManager.instance.GameOver();
        }
    }

    void LaserOff() {
        mainLaser.SetPosition(0, Vector3.zero);
        mainLaser.SetPosition(1, Vector3.zero);
        rightLaser.SetPosition(0, Vector3.zero);
        rightLaser.SetPosition(1, Vector3.zero);
        leftLaser.SetPosition(0, Vector3.zero);
        leftLaser.SetPosition(1, Vector3.zero);
    }
}