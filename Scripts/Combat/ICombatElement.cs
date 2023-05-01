using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatElement:IOwnership{
    void RemoveHp(ICombatElement e, int hpToRemove);
    void AddHp(int hpToAdd);
    void SetHpSliderActive(bool active);
    void OnDeath();
    IEnumerator Attack();
    void SetTarget();

    void CombatElementDied(ICombatElement combatElement);
    
    ICombatElement GetTarget();

    void AddCombatElementToVision(ICombatElement combatElement);
    void RemoveCombatElementFromVision(ICombatElement combatElement);
    
    
    new GameObject GetGameObject();
    new Player GetPlayer();

    float GetCurrentHP();
    float GetMaxHP();

}
