using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class C_Combat : MonoBehaviour{
    public ProjectorController projectorController;
    [SerializeField] private int range;
    [SerializeField] private Vision vision;
    [SerializeField] private float attackTime;
    [SerializeField] private int attackDmg;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform bulletStart;
    [SerializeField] private List<C_Health> cHealthsInVision  = new List<C_Health>();
    [SerializeField] private C_Health target;
    private Entity entity;
    [SerializeField] private C_Weapon CWeapon;
    private static readonly int PulledSword = Animator.StringToHash("PulledSword");

    void Awake(){
        entity = gameObject.GetComponent<Entity>();
    }

    void Start(){
        if (CWeapon != null) CWeapon.SetEntity(entity);
        vision.UpdateRange(range);
        EventManager.Instance.deathEvent.AddListener(EntityDied);
        EventManager.Instance.setTarget.AddListener(SetSpecificTarget);
        projectorController.circleRadius = range * entity.transform.localScale.x;
        projectorController.circleColor = entity.GetPlayer().PlayerColor;
        projectorController.UpdateMaterialProperties();
    }

    

    private IEnumerator Attack(){
        MoveToTarget();
        if (entity.Animator != null){
            entity.Animator.SetBool(PulledSword, true);
            if (CWeapon != null) CWeapon.SetWeaponActive(true);
        }
        if (cHealthsInVision.Contains(target)){
            float timeTillAttack = attackTime;
            if (CWeapon != null){
                CWeapon.Attack();
            }
            
            
            //ShootBullet();
            while (timeTillAttack > 0){
                yield return null; // wait for the next frame
                timeTillAttack -= Time.deltaTime;
            }

            if (target != null){
                StartCoroutine(Attack());
            }
        }
        else{
            MoveToTarget();
        }
    }
    
    private void ShootBullet(){
        Transform t = transform;
        GameObject projectileObject = Instantiate(projectile, t.position, t.rotation);
        Projectile bullet = projectileObject.GetComponent<Projectile>();
        bullet.StartPos = bulletStart.position;
        bullet.Owner = this;
        bullet.Target = target.Entity;
        bullet.Damage = attackDmg;
    }

    private void MoveToTarget(){
        if (entity.CMoveable != null){
            entity.CMoveable.SetMoveToPosition(target.transform.position,false);
        }
    }

    private void SetSpecificTarget(C_Combat attacker, C_Health target){
        if (attacker == this){
            this.target = target;
            if (this.target != null){
                StartCoroutine(Attack());
            }
        }
    }
    
    public void SetTarget(){
        if (cHealthsInVision.Count == 0){
            if (entity.Animator != null){
                entity.Animator.SetBool(PulledSword, false);
                if (CWeapon != null) CWeapon.SetWeaponActive(false);
            }
            target = null;
            return;
        }

        foreach (C_Health targetCHealth in cHealthsInVision){
            if (targetCHealth.Entity.GetPlayer() != entity.GetPlayer() && entity.GetPlayer()!=null){
                target = targetCHealth;
                break;
            }
        }
        if (target != null){
            StartCoroutine(Attack());
        }
    }

    private void EntityDied(C_Health cHealth){
        
        if (cHealthsInVision.Contains(cHealth)){
            RemoveCHealthFromVision(cHealth);
            if (target == cHealth){
                target = null;
                SetTarget();
            }
        }
    }


    public C_Health GetTarget(){
        return target;
    }

    public void AddCHealthToVision(C_Health cHealth){
        cHealthsInVision.Add(cHealth);
    }

    public void RemoveCHealthFromVision(C_Health cHealth){
        cHealthsInVision.Remove(cHealth);
    }

    public int GetAttackDmg(){
        return attackDmg;
    }
    
    
}
