using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Search;

public class Entity : MonoBehaviour{
    [SerializeField] private int hp;
    private Vision vision;
    [SerializeField] private List<Entity> entitiesInVision  = new List<Entity>();
    [SerializeField] private Entity target;
    [SerializeField] private float attackTime;
    [SerializeField] private int attackDmg;
    [SerializeField] private GameObject projectile;
    public Material material;
    private bool isAttacking = false;

    public Vector3 GetPosition(){
        return transform.position;
    }

    public void RemoveHp(int hpToRemove){
        hp -= hpToRemove;
    }
    
    public void AddHp(int hpToAdd){
        hp += hpToAdd;
    }

    protected void Awake(){
        vision = transform.GetChild(0).GetComponent<Vision>();
        
    }

    protected void Start(){
        
    }
    
    protected void Update(){
        if (hp <= 0){
            Destroy(gameObject);
        }
        if (target != null && !isAttacking){
            StartCoroutine(Attack());
        }
    }
    

    private IEnumerator Attack(){
        float timeTillAttack = attackTime;
        isAttacking = true;
    
        GameObject projectileObject = Instantiate(projectile, transform.position, transform.rotation);
        Projectile bullet = projectileObject.GetComponent<Projectile>();
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
}