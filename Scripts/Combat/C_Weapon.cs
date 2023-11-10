using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class C_Weapon : MonoBehaviour{
    protected GameObject weapon;
    protected Entity entity;
    [SerializeField] protected int attackDmg;
    [SerializeField] protected int attackRange;
    public static readonly int AnimAttack = Animator.StringToHash("Attack");
    protected virtual void Awake(){
        weapon = gameObject;
    }

    protected virtual void Start(){
        weapon.SetActive(false);
    }

    public virtual void Attack(){
        
    }

    public void SetWeaponActive(bool active){
        weapon.SetActive(active);
    }

    public void SetEntity(Entity e){
        entity = e;
    }

    public int GetAttackRange(){
        return attackRange;
    }

    public int GetAttackDamage(){
        return attackDmg;
    }

}
