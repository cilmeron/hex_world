using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Health : MonoBehaviour{
    [SerializeField] public float CurrentHP;
    [SerializeField] public float MaxHP;
    [SerializeField] private HpSlider hpSlider;
    [SerializeField] private Entity entity;

    void Awake(){
        entity = GetComponent<Entity>();
    }
    

    private void Start(){
        EventManager.Instance.damageEvent.AddListener(RemoveHp);
        EventManager.Instance.deathEvent.AddListener(OnDeath);
        hpSlider.UpdateHpSlider();
    }

    private void RemoveHp(C_Health cHealth,int hpToRemove){
        if (this != cHealth) return;
        CurrentHP -= hpToRemove;
        hpSlider.UpdateHpSlider();
        if (CurrentHP > 0) return;
        EventManager.Instance.deathEvent.Invoke(this);
    }
        
        public void AddHp(int hpToAdd){
            CurrentHP += hpToAdd;
            if (CurrentHP > MaxHP){
                CurrentHP = MaxHP;
            }
            hpSlider.UpdateHpSlider();
        }
        public void SetHpSliderActive(bool active){
            hpSlider.Activate(active);
        }

        private void OnDeath(C_Health cHealth){
            if (cHealth != this) return;
            if (entity.CMoveable != null){
                entity.CMoveable.NavMeshAgent.enabled = false;
                entity.collider.direction = 2;
            }
            Destroy(gameObject,10);
        }
       
        public float GetCurrentHp(){
            return CurrentHP;
        }

        public float GetMaxHp(){
            return MaxHP;
        }

        public Entity Entity{
            get => entity;
        }
}
