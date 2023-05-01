using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface ISelectable{
    float GetHpPercentage();
    Sprite GetSprite();
    String GetStats();
    GameObject GetGameObject();
    Vector3 GetPosition();
    
    bool IsFormationElement(){
        return this is IFormationElement;
    }
    
    
    SelectableMas GetSelectableMas();
    

    [CanBeNull]
    IFormationElement GetFormationElement(){
        return GetGameObject().GetComponent<IFormationElement>();
    }
    bool IsSelected();

    bool IsCombatElement(){
        return this is ICombatElement;
    }

    [CanBeNull]
    ICombatElement GetCombatElement(){
        return GetGameObject().GetComponent<ICombatElement>();
    }
}