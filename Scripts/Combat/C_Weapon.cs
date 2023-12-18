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
    [SerializeField] protected int attackSpeed;
    public static readonly int AnimAttack = Animator.StringToHash("Attack");
    private GameManager.GameDifficulty difficulty;
    protected virtual void Awake(){
        weapon = gameObject;
    }

    protected virtual void Start(){
        weapon.SetActive(false);
        difficulty = GameManager.Instance.GetGameDifficulty();

    }

    public virtual void Attack(C_Health target,Entity owner){
        
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

    public int GetAttackSpeed(){
        return attackSpeed;
    }

    public void OnTriggerEnter(Collider other){
        if (entity == null) return;
        if (other.gameObject.GetComponent<Entity>() != null){
            Entity hitEntity = other.gameObject.GetComponent<Entity>();
            try{
                if (difficulty == GameManager.GameDifficulty.Easy &&
                    hitEntity.GetNation() == entity.GetNation()) return;
            }
            catch (Exception e){
                Debug.Log("here");
            }
            if (difficulty == GameManager.GameDifficulty.Easy && hitEntity.GetNation() == entity.GetNation()) return;
            if (hitEntity.CHealth != null && hitEntity.CHealth.GetCurrentHp()>0){
                EventManager.Instance.damageEvent.Invoke(hitEntity.CHealth,attackDmg);
            }
        }
    }
}
