using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Ninjato : C_Weapon{

    private MeshCollider _weaponCollider;
    
    protected override void Awake(){
        base.Awake();
        _weaponCollider = GetComponent<MeshCollider>();
    }

    public override void Attack(C_Health target,Entity owner){
        base.Attack(target,owner);
        entity.Animator.SetTrigger(AnimAttack);
        Debug.Log("attacked");
    }
    

}