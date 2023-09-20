using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class SelectionComponent : MonoBehaviour{
    private C_Selectable selectable;
    private Entity entity;
    
    void Start(){
        
        selectable = GetComponent<C_Selectable>();
        if (selectable == null){
            throw new Exception("Selectable is non existing");
        }
        if (selectable.Entity == null){
            throw new Exception("Selectable has no Entity");
        }
        entity = selectable.Entity;
        entity.SetMaterialToRenderer(entity.CLook.MSelected);
        if (entity.CHealth != null){
            entity.CHealth.SetHpSliderActive(true);
            
        }
        if (entity.CCombat!=null){
            entity.CCombat.projectorController.gameObject.SetActive(true);
        }
    }

    private void OnDestroy(){
        if (selectable.Entity.CHealth != null){
            selectable.Entity.CHealth.SetHpSliderActive(false);
        }
        if (selectable.Entity.CCombat!=null){
            selectable.Entity.CCombat.projectorController.gameObject.SetActive(false);
        }

        C_Formation formation = entity.CFormation;
        if (formation != null &&  formation.IsInFormation() && formation.IsLeader()){
            entity.SetMaterialToRenderer(entity.CLook.MLeader);
        }
        else{
            entity.SetMaterialToRenderer(entity.CLook.MUnselected);
        }
    }
}