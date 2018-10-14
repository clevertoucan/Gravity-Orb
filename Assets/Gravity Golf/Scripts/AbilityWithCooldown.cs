using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityWithCooldown : MonoBehaviour {
    public float effectDuration, cooldown;
    protected float startTime = 0, endTime = 0;
    public Image abilityRootIcon, abilityProgressIcon;
    public Abilities.CombatAbility combatAbility;
    public Abilities.EvasionAbility evasionAbility;
    Abilities abilities;

    protected virtual void Start() {
        abilities = Abilities.instance;
        if (( abilities.CurrentCombatAbility != Abilities.CombatAbility.None && abilities.CurrentCombatAbility == combatAbility ) ||
            ( abilities.CurrentEvasionAbility != Abilities.EvasionAbility.None && abilities.CurrentEvasionAbility == evasionAbility )) {
            abilityRootIcon.gameObject.SetActive(true);
        } else {
            abilityRootIcon.gameObject.SetActive(false);
        }
    }


    public void ActivateAbility() {
        StartCoroutine(Ability());
    }

    public virtual void StartAbility() {}

    public virtual void StopAbility() {}

    protected IEnumerator Flash(Image flash, float flashDuration) {
        flash.enabled = true;
        float startTime = Time.time, percentage = 0;
        while (percentage < 1) {
            percentage = ( Time.time - startTime ) / flashDuration;
            flash.color = new Color(flash.color.r, flash.color.g, flash.color.b, 1 - percentage);
            yield return null;
        }
        flash.enabled = false;
    }

    IEnumerator Ability() {
        if (endTime < Time.unscaledTime - cooldown) {
            StartAbility();
            startTime = Time.unscaledTime;
            while (Time.unscaledTime - startTime < effectDuration) {
                abilityProgressIcon.fillAmount = 1 - ( Time.unscaledTime - startTime ) / effectDuration;
                yield return null;
            }
            abilityProgressIcon.fillAmount = 0;
            Time.timeScale = 1;
            endTime = Time.unscaledTime;
            StopAbility();
            while (Time.unscaledTime < endTime + cooldown) {
                abilityProgressIcon.fillAmount = ( Time.unscaledTime - endTime ) / cooldown;
                yield return null;
            }
            abilityProgressIcon.fillAmount = 1;
        } else {
            abilityProgressIcon.fillAmount = ( Time.unscaledTime - endTime ) / cooldown;
        }
    }
}
