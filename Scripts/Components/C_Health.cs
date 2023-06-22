using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Health : MonoBehaviour{
    [SerializeField] private float currentHP;
    [SerializeField] private float maxHP;
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
        currentHP -= hpToRemove;
        hpSlider.UpdateHpSlider();
        if (currentHP > 0) return;
        EventManager.Instance.deathEvent.Invoke(this);
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

        private void OnDeath(C_Health cHealth){
            if (cHealth != this) return;
            Destroy(gameObject);
        }
       
        public float GetCurrentHp(){
            return currentHP;
        }

        public float GetMaxHp(){
            return maxHP;
        }

        public Entity Entity{
            get => entity;
        }
}
