using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Sword : C_Weapon{

    private MeshCollider _weaponCollider;
    
    protected override void Awake(){
        base.Awake();
        _weaponCollider = GetComponent<MeshCollider>();
    }

    public override void Attack(){
        base.Attack();
        entity.Animator.SetTrigger(AnimAttack);
        Debug.Log("attacked");
    }
    

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.GetComponent<Entity>() != null){
            Entity hitEntity = other.gameObject.GetComponent<Entity>();
            if (hitEntity.CHealth != null && hitEntity.CHealth.GetCurrentHp()>0){
                EventManager.Instance.damageEvent.Invoke(hitEntity.CHealth,attackDmg);
            }
        }
    }
}
