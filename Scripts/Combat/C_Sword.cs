using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Sword : C_Weapon{

    private MeshCollider collider;
    
    protected override void Awake(){
        base.Awake();
        collider = GetComponent<MeshCollider>();
    }

    public override void Attack(){
        base.Attack();
        entity.Animator.SetBool(AnimAttack, true);
        StartCoroutine(ResetAttackFlag());
    }
    
    private IEnumerator ResetAttackFlag()
    {
        // Wait for half a second (0.5 seconds).
        yield return new WaitForSeconds(5f);
        entity.Animator.SetBool(AnimAttack, false);
        
    }

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.GetComponent<Entity>() != null){
            Entity hitEntity = other.gameObject.GetComponent<Entity>();
            if (hitEntity.CHealth != null){
                EventManager.Instance.damageEvent.Invoke(hitEntity.CHealth,entity.CCombat.GetAttackDmg());
                Debug.Log(entity.gameObject.name + " has hit + " + hitEntity.gameObject.name);
            }
        }
    }

}
