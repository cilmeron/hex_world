using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class SelectionComponent : MonoBehaviour{
    private C_Selectable selectable;
    
    void Start(){
        
        selectable = GetComponent<C_Selectable>();
        if (selectable == null){
            throw new Exception("Selectable is non existing");
        }
        if (selectable.Entity == null){
            throw new Exception("Selectable has no Entity");
        }
        //entity.SetMaterialToRenderer(entity.CLook.MSelected);
        if (selectable.Entity.CHealth != null){
            selectable.Entity.CHealth.SetHpSliderActive(true);
            
        }
        selectable.Entity.detector.EnableProjector(true);
        if (selectable.Entity.CCombat != null){
            selectable.Entity.CCombat._attackDistanceDetector.EnableProjector(true);
        }
    }

    private void OnDestroy(){
        if (selectable.Entity.CHealth != null){
            selectable.Entity.CHealth.SetHpSliderActive(false);
        }
        selectable.Entity.detector.EnableProjector(false);
        if (selectable.Entity.CCombat != null){
            selectable.Entity.CCombat._attackDistanceDetector.EnableProjector(false);
        }
    }
}