using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities: MonoBehaviour {
    public enum EvasionAbility {
        None, Scrambler, Recall, GravityControl
    }
    public enum CombatAbility {
        None, BulletTime, Needle, GrapplingHook
    }
    private EvasionAbility currentEvasionAbility = EvasionAbility.None;
    private CombatAbility currentCombatAbility = CombatAbility.None;
    public EvasionAbility CurrentEvasionAbility {
        get {
            return currentEvasionAbility;
        }
        set {
            currentEvasionAbility = value;
            persistance.WriteData("currentEvasionAbility", currentEvasionAbility);
        }
    }
    public CombatAbility CurrentCombatAbility {
        get {
            return currentCombatAbility;
        }
        set {
            currentCombatAbility = value;
            persistance.WriteData("currentCombatAbility", currentCombatAbility);
        }
    }
    public List<EvasionAbility> purchasedEvasionAbilities;
    public List<CombatAbility> purchasedCombatAbilities;
    public static Abilities instance;
    Persistance persistance;

    public delegate void OnAbilitiesUpdatedAction();
    public static event OnAbilitiesUpdatedAction OnAbilitiesUpdated;

    public void AddPurchasedEvasionAbility(EvasionAbility e) {
        purchasedEvasionAbilities.Add(e);
        persistance.WriteData("purchasedEvasionAbilities", purchasedEvasionAbilities);
    }

    public void AddPurchasedCombatAbility(CombatAbility c) {
        purchasedCombatAbilities.Add(c);
        persistance.WriteData("purchasedCombatAbilities", purchasedCombatAbilities);
    }

    private void Awake() {
        instance = this;
    }

    private void Start() {
        persistance = Persistance.instance;
        UpdateAbilities();
    }

    public void UpdateAbilities() {
        currentEvasionAbility = persistance.ReadData("currentEvasionAbility", EvasionAbility.None);
        currentCombatAbility = persistance.ReadData("currentCombatAbility", CombatAbility.None);
        purchasedEvasionAbilities = persistance.ReadData("purchasedEvasionAbilities", new List<EvasionAbility>());
        purchasedCombatAbilities = persistance.ReadData("purchasedCombatAbilities", new List<CombatAbility>());
        if (OnAbilitiesUpdated != null) {
            OnAbilitiesUpdated();
        }
    }

    private void OnDestroy() {
        OnAbilitiesUpdated = null;
    }

}
