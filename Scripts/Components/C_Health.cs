using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Health : MonoBehaviour, Detectable{
    [SerializeField] private float currentHP;
    [SerializeField] private float maxHP;
    [SerializeField] private HpSlider hpSlider;
    [SerializeField] private Entity entity;

    void Awake(){
        entity = GetComponent<Entity>();
    }
    

    private void Start(){
        EventManager.Instance.damageEvent.AddListener(RemoveHp);
        hpSlider.UpdateHpSlider();
    }

    private void RemoveHp(C_Health cHealth,int hpToRemove){
        if (this != cHealth) return;
        currentHP -= hpToRemove;
        hpSlider.UpdateHpSlider();
        if (currentHP > 0) return;
        EventManager.Instance.deathEvent.Invoke(this);
        entity.DestroyEntity();
        Destroy(gameObject,10);

    }
        
        public void AddHp(int hpToAdd){
            currentHP += hpToAdd;
            if (currentHP > maxHP){
                currentHP = maxHP;
            }
            hpSlider.UpdateHpSlider();
        }
        public void SetHpSliderActive(bool active){
            hpSlider.Activate(active);
        }

        private void OnDeath(){
            if (entity.CMoveable != null){
                entity.CMoveable.NavMeshAgent.enabled = false;
                entity.collider.direction = 2;
            }

            if (entity.CCombat != null){
                entity.CCombat.enabled = false;
            }
            entity.DestroyEntity();
            Destroy(gameObject,10);
            
        }
       
        public float GetCurrentHp(){
            return currentHP;
        }

        public float GetMaxHp(){
            return maxHP;
        }

        public bool IsAlive(){
            return currentHP >= 0;
        }
        
        public Entity Entity{
            get => entity;
        }
}
