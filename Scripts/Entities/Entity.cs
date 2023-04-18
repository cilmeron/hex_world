using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Search;

public class Entity : MonoBehaviour{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int currentHP;
    private Vision vision;
    [SerializeField] private List<Entity> entitiesInVision  = new List<Entity>();
    [SerializeField] private Entity target;
    [SerializeField] private float attackTime;
    [SerializeField] private int attackDmg;
    [SerializeField] private GameObject projectile;
    public Material material;
    public Material selectedMaterial;
    private bool isAttacking = false;
    private HpSlider hpSlider;
    [SerializeField] private Transform bulletStart;
    [SerializeField] protected Player player;

    public Vector3 GetPosition(){
        return transform.position;
    }

    public void RemoveHp(Entity e,int hpToRemove){
        if (this == e){
            currentHP -= hpToRemove;
        }
    }
    
    public void AddHp(int hpToAdd){
        currentHP += hpToAdd;
    }

    protected void Awake(){
        vision = transform.GetChild(0).GetComponent<Vision>();
        hpSlider = transform.GetChild(1).GetComponent<HpSlider>();
    }

    protected void Start(){
        EventManager.Instance.deathEvent.AddListener(SetNewTarget);
        EventManager.Instance.damageEvent.AddListener(RemoveHp);
    }
    
    protected void Update(){
        if (currentHP <= 0){
            OnDeath();
        }

        if (target != null && !isAttacking){
            StartCoroutine(Attack());
        }
    }

    private void SetNewTarget(Entity e){
        if (entitiesInVision.Contains(e)){
            entitiesInVision.Remove(e);
        }
        if (target==null || target != e){
            return;
        }
        if (entitiesInVision.Count == 0){
            target = null;
            return;
        }
        target = entitiesInVision[0];
    }


    private IEnumerator Attack(){
        float timeTillAttack = attackTime;
        isAttacking = true;
    
        GameObject projectileObject = Instantiate(projectile, transform.position, transform.rotation);
        Projectile bullet = projectileObject.GetComponent<Projectile>();
        bullet.StartPos = bulletStart.position;
        bullet.Owner = this;
        bullet.Target = target;
        bullet.Damage = attackDmg;
    
        while (timeTillAttack > 0){
            yield return null; // wait for the next frame
            timeTillAttack -= Time.deltaTime;
        }
        isAttacking = false;
    }

    public List<Entity> EntitiesInVision{
        get => entitiesInVision;
        set => entitiesInVision = value;
    }

    public Entity Target{
        get => target;
        set => target = value;
    }

    public int MAXHp{
        get => maxHP;
        set => maxHP = value;
    }

    public int CurrentHp{
        get => currentHP;
        set => currentHP = value;
    }

    public void SetHpSliderActive(bool active){
        hpSlider.IsActive = active;
    }

    protected virtual void OnDeath(){
        EventManager.Instance.deathEvent.Invoke(this);
        Destroy(gameObject);
    }

    public void SetPlayer(Player p){
        player = p;
    }

    public Player GetPlayer(){
        return player;
    }
    
}