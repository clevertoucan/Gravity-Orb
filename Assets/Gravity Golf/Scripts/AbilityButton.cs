using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour {
    static AbilityButton highlightedButton;
    public MaskableGraphic foreground, background, selected;
    bool switched;
    public delegate void OnClickAction(AbilityButton a);
    public static event OnClickAction OnClick;
    ColorController c;
    public string title, description;
    public int scoreCost;
    public Abilities.CombatAbility combatAbility;
    public Abilities.EvasionAbility evasionAbility;
    Abilities abilities;

    private void Awake() {
        Abilities.OnAbilitiesUpdated += CheckCurrentAbility;
        DescriptionController.OnEquip += CheckCurrentAbility;
    }

    // Use this for initialization
    void Start () {
        abilities = Abilities.instance;
        c = ColorController.instance;
        OnClick += ResetColors;
        ColorController.OnColorSet += SetColor;
	}

    void SetColor(Color themeColor, Color backgroundColor, Color foregroundColor, Color accentColor) {
        if (!switched) {
            foreground.color = Color.white;
            background.color = accentColor;
        } else {
            foreground.color = accentColor;
            background.color = Color.white;
        }
    }

    public void CheckCurrentAbility() {
        if (combatAbility != Abilities.CombatAbility.None && Abilities.instance.CurrentCombatAbility == combatAbility) {
            selected.gameObject.SetActive(true);
        } else if (evasionAbility != Abilities.EvasionAbility.None && Abilities.instance.CurrentEvasionAbility == evasionAbility) {
            selected.gameObject.SetActive(true);
        } else {
            selected.gameObject.SetActive(false);
        }
    }

    public void Select() {
        OnClick(this);
        switched = true;

    }

    private void ResetColors(AbilityButton a) {
        switched = false;
    }

    private void OnDestroy() {
        OnClick = null;
    }
}
