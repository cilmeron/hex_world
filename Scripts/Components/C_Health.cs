using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Health : MonoBehaviour, Detectable
{
    [SerializeField] public float CurrentHP;
    [SerializeField] private float maxHP;
    [SerializeField] private HpSlider hpSlider;
    private Entity owner;

    public int GetID()
    {
        return owner.ID;
    }
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
        owner.removehp(hpToRemove, (int)CurrentHP);
        return;
     //   if (this != cHealth) return;
        //CurrentHP -= hpToRemove;
        //hpSlider.UpdateHpSlider();
        //if (CurrentHP > 0) return;
        //owner.DestroyEntity();
        //Destroy(gameObject, 10);

    }

    public void AddHp(int hpToAdd)
    {
        CurrentHP += hpToAdd;
        if (CurrentHP > maxHP)
        {
            CurrentHP = maxHP;
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
            owner.Collider.direction = 2;
        }

        if (owner.CCombat != null)
        {
            owner.CCombat.enabled = false;
        }
        owner.DestroyEntity();
        Destroy(gameObject, 10);

    }

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
