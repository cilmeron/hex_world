using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD
public class C_Health : MonoBehaviour{
    [SerializeField] public float CurrentHP;
    [SerializeField] public float MaxHP;
=======
public class C_Health : MonoBehaviour, Detectable
{
    [SerializeField] public float CurrentHP;
    [SerializeField] private float maxHP;
>>>>>>> TowerBehaviour
    [SerializeField] private HpSlider hpSlider;
    private Entity owner;

    void Awake()
    {
        owner = GetComponent<Entity>();
    }

    private void Start()
    {
        EventManager.Instance.damageEvent.AddListener(RemoveHp);
        hpSlider.UpdateHpSlider();
    }

    private void RemoveHp(C_Health cHealth, int hpToRemove)
    {
        if (this != cHealth) return;
        CurrentHP -= hpToRemove;
        hpSlider.UpdateHpSlider();
        if (CurrentHP > 0) return;
<<<<<<< HEAD
=======
        Debug.Log("Dead");
>>>>>>> TowerBehaviour
        EventManager.Instance.deathEvent.Invoke(this);
        owner.DestroyEntity();
        Destroy(gameObject, 10);

    }
<<<<<<< HEAD
        
        public void AddHp(int hpToAdd){
            CurrentHP += hpToAdd;
            if (CurrentHP > MaxHP){
                CurrentHP = MaxHP;
            }
            hpSlider.UpdateHpSlider();
=======

    public void AddHp(int hpToAdd)
    {
        CurrentHP += hpToAdd;
        if (CurrentHP > maxHP)
        {
            CurrentHP = maxHP;
>>>>>>> TowerBehaviour
        }
        hpSlider.UpdateHpSlider();
    }
    public void SetHpSliderActive(bool active)
    {
        hpSlider.Activate(active);
    }

    private void OnDeath()
    {
        if (owner.CMoveable != null)
        {
            owner.CMoveable.NavMeshAgent.enabled = false;
        }

<<<<<<< HEAD
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
=======
        if (owner.CCombat != null)
        {
            owner.CCombat.enabled = false;
>>>>>>> TowerBehaviour
        }
        owner.DestroyEntity();
        Destroy(gameObject, 10);

<<<<<<< HEAD
        public float GetMaxHp(){
            return MaxHP;
        }
=======
    }
>>>>>>> TowerBehaviour

    public float GetCurrentHp()
    {
        return CurrentHP;
    }

    public float GetMaxHp()
    {
        return maxHP;
    }

    public bool IsAlive()
    {
        return CurrentHP >= 0;
    }

    public Entity Entity
    {
        get => owner;
    }
}
