using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Health : MonoBehaviour, Detectable{
    [SerializeField] private float currentHP;
    [SerializeField] private float maxHP;
    [SerializeField] private HpSlider hpSlider;
     private Entity owner;

    void Awake(){
        owner = GetComponent<Entity>();
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
        owner.DestroyEntity();
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
            if (owner.CMoveable != null){
                owner.CMoveable.NavMeshAgent.enabled = false;
                owner.collider.direction = 2;
            }

            if (owner.CCombat != null){
                owner.CCombat.enabled = false;
            }
            owner.DestroyEntity();
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
            get => owner;
        }
}
