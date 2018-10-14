using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scrambler : AbilityWithCooldown {
    bool onCooldown = false;
    public Image flashScreen;
    public float flashDuration;

    public override void StartAbility() {
        EnemyController.scrambled = true;
        StartCoroutine(Flash(flashScreen, flashDuration));
    }

    public override void StopAbility() {
        EnemyController.scrambled = false;
    }
}
