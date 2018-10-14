using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionController : MonoBehaviour {
    public Text fillerText, title, description, scoreCost;
    public Button buyButton, equipButton, unequipButton;
    Abilities abilities;
    AbilityButton selectedAbility;
    public delegate void OnEquipAction();
    public static event OnEquipAction OnEquip;
    // Use this for initialization
    void Start () {
        AbilityButton.OnClick += OnAbilitySelect;
        OnAbilityDeselect();
        abilities = Abilities.instance;
	}

    void OnAbilitySelect(AbilityButton a) {
        selectedAbility = a;
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
        scoreCost.gameObject.SetActive(false);
        fillerText.gameObject.SetActive(false);
        title.gameObject.SetActive(true);
        description.gameObject.SetActive(true);
        title.text = a.title;
        description.text = a.description; 
        if (a.combatAbility != Abilities.CombatAbility.None) {
            if (abilities.purchasedCombatAbilities.Contains(a.combatAbility)) {
                if (abilities.CurrentCombatAbility == selectedAbility.combatAbility) {
                    unequipButton.gameObject.SetActive(true);
                } else {
                    equipButton.gameObject.SetActive(true);
                }
                scoreCost.gameObject.SetActive(false);
            } else {
                buyButton.gameObject.SetActive(true);
                scoreCost.gameObject.SetActive(true);
                scoreCost.text = a.scoreCost.ToString();
            }
        }
        if (a.evasionAbility != Abilities.EvasionAbility.None) {
            if (abilities.purchasedEvasionAbilities.Contains(a.evasionAbility)) {
                if (abilities.CurrentEvasionAbility == selectedAbility.evasionAbility) {
                    unequipButton.gameObject.SetActive(true);
                } else {
                    equipButton.gameObject.SetActive(true);
                }
                scoreCost.gameObject.SetActive(false);
            } else {
                buyButton.gameObject.SetActive(true);
                scoreCost.gameObject.SetActive(true);
                scoreCost.text = a.scoreCost.ToString();
            }
        }
    }

    public void Buy() {
        if (!abilities.purchasedCombatAbilities.Contains(selectedAbility.combatAbility)) {
            Score.instance.BuyAbility(selectedAbility.scoreCost);
            abilities.AddPurchasedCombatAbility(selectedAbility.combatAbility);
            OnAbilitySelect(selectedAbility);
        } else if (!abilities.purchasedEvasionAbilities.Contains(selectedAbility.evasionAbility)) {
            Score.instance.BuyAbility(selectedAbility.scoreCost);
            abilities.AddPurchasedEvasionAbility(selectedAbility.evasionAbility);
            OnAbilitySelect(selectedAbility);
        }
    }

    public void Unequip() {
        if (selectedAbility.combatAbility != Abilities.CombatAbility.None) {
            abilities.CurrentCombatAbility = Abilities.CombatAbility.None;
        } else if (selectedAbility.evasionAbility != Abilities.EvasionAbility.None) {
            abilities.CurrentEvasionAbility = Abilities.EvasionAbility.None;
        }
        OnAbilitySelect(selectedAbility);
        if (OnEquip != null) {
            OnEquip();
        }
    }

    public void Equip() {
        if ( abilities.purchasedCombatAbilities.Contains(selectedAbility.combatAbility) && selectedAbility.combatAbility != Abilities.CombatAbility.None) {
            abilities.CurrentCombatAbility = selectedAbility.combatAbility;
            OnAbilitySelect(selectedAbility);
        } else if (abilities.purchasedEvasionAbilities.Contains(selectedAbility.evasionAbility) && selectedAbility.evasionAbility != Abilities.EvasionAbility.None) {
            abilities.CurrentEvasionAbility = selectedAbility.evasionAbility;
            OnAbilitySelect(selectedAbility);
        }

        if (OnEquip != null) {
            OnEquip();
        }
    }

    void OnAbilityDeselect() {
        fillerText.gameObject.SetActive(true);
        title.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void OnDestroy() {
        OnEquip = null;
    }
}
