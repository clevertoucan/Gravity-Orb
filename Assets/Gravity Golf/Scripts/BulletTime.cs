using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletTime : AbilityWithCooldown {
    PlayerController player;
    [Range(0, 1)]
    public float timeScale;
    float regularfixedTimeStep, fixedTimeStep;
    private void Awake() {
        player = GetComponent<PlayerController>();
        regularfixedTimeStep = Time.fixedDeltaTime;
        fixedTimeStep = regularfixedTimeStep * timeScale;
    }
    public override void StartAbility() {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = fixedTimeStep;
        player.fireMode = PlayerController.FireMode.bulletTime;
    }

    public override void StopAbility() {
        Time.timeScale = 1;
        Time.fixedDeltaTime = regularfixedTimeStep;
        player.fireMode = PlayerController.FireMode.regular;
    }


}
