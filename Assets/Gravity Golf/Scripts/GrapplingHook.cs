using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : AbilityWithCooldown {
    PlayerController player;
    [Range(0, 1)]
    public float timeScale;
    protected override void Start() {
        base.Start();
        player = GetComponent<PlayerController>();
    }
    public override void StartAbility() {
        Time.timeScale = timeScale;
        player.fireMode = PlayerController.FireMode.grapplingHook;
    }

    public override void StopAbility() {
        Time.timeScale = 1;
        player.fireMode = PlayerController.FireMode.regular;
    }
}
