using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotBossController : EnemyController{
    public ScattershotBossNode right, left;
    public float area = 3;

    protected new void Start() {
        base.Start();
    }

    float rot = 0, percentage = 0, startTime = 0;
    protected void Update() {
        isFiring = left.isFiring || right.isFiring;
        if (Time.time - startTime > cooldown) {
            startTime = Time.time;
        }
        percentage = ( Time.time - startTime ) / cooldown;
        rot = Mathf.Lerp(0, 360, percentage);
        transform.LookAt(player.transform);
        transform.Rotate(Vector3.up, rot);
        if (Aggro()) {
            if (!left.isFiring && ( rot < area || rot > 360 - area )) {
                StartCoroutine(left.FireSequence());
            } else if (!right.isFiring && ( rot < 180 + area && rot > 180 - area )){
                StartCoroutine(right.FireSequence());
            }
        }
    }
}
