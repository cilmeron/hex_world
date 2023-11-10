using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Combat : MonoBehaviour, DetectorNotification{
    [SerializeField] private float attackTime;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform bulletStart;
    [SerializeField] private List<C_Health> cHealthsInAttackRange  = new List<C_Health>();
    [SerializeField] private C_Health target;
    private Entity owner;
    [SerializeField] private C_Weapon CWeapon;
    private static readonly int PulledSword = Animator.StringToHash("PulledSword");
    [SerializeField] private float timeTillAttack = 0f;
    [SerializeField] public Detector _attackDistanceDetector;
    private float timeTillTargetRotationCheck = 0f;
    

    void Awake(){
        owner = gameObject.GetComponent<Entity>();
        _attackDistanceDetector.SetOwner(owner);
    }

    void Start(){
        if (CWeapon != null) CWeapon.SetEntity(owner);
        EventManager.Instance.deathEvent.AddListener(EntityDied);
        EventManager.Instance.setTarget.AddListener(SetSpecificTarget);
        _attackDistanceDetector.SetDetectorNotification(this);
        _attackDistanceDetector.SetRadius(CWeapon.GetAttackRange());
    }

    private IEnumerator CheckTargetRotation(){
        if (target != null){
            timeTillTargetRotationCheck = 3f;
            while (timeTillTargetRotationCheck > 0){
                yield return null; // wait for the next frame
                timeTillTargetRotationCheck -= Time.deltaTime;
            }
            if (target != null){
                StartCoroutine(TurnToTarget());
            }

            StartCoroutine(CheckTargetRotation());

        }
    }
    private IEnumerator TurnToTarget(){
        int rotationSpeed = 20;
        if (target != null)
        {
            // Calculate the direction to the target
            Vector3 targetDirection = target.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            while (transform.rotation != targetRotation)
            {
                float step = rotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
                yield return null;
            }
        }
    }

    private IEnumerator Attack(){
        if (cHealthsInAttackRange.Contains(target)){
            timeTillAttack = attackTime;
            if (CWeapon != null){
                CWeapon.Attack();
            }
            while (timeTillAttack > 0){
                yield return null; // wait for the next frame
                timeTillAttack -= Time.deltaTime;
            }
            if (target != null){
                StartCoroutine(Attack());
            }
        }else{
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
        bullet.Damage = CWeapon.GetAttackDamage();
    }

    private void MoveToTarget(){
        if (owner.CMoveable != null){
            owner.CMoveable.SetMoveToTransform(target.transform,false);
        }
    }

    private void SetSpecificTarget(C_Combat attacker, C_Health target){
        if (attacker == this){
            this.target = target;
            if (owner.Animator != null){
                owner.Animator.SetBool(PulledSword, true);
                if (CWeapon != null) CWeapon.SetWeaponActive(true);
            }
            MoveToTarget();
        }
    }

    private void ResetTarget(){
        target = null;
        if (owner.Animator != null){
            owner.Animator.SetBool(PulledSword, false);
            owner.Animator.SetBool(C_Weapon.AnimAttack, false);
            if (CWeapon != null) CWeapon.SetWeaponActive(false);
        }
        StopCoroutine(CheckTargetRotation());
    }
    public void SetTarget(){
        if (target != null) return;
        if (owner._entitiesInVision.Count == 0){
            ResetTarget();
            return;
        }

        if (cHealthsInAttackRange.Count > 0){
            foreach (C_Health cHealth in cHealthsInAttackRange){
                if (cHealth == null) continue;
                if (cHealth.Entity.GetPlayer() != owner.GetPlayer() && owner.GetPlayer()!=null){
                    target = cHealth;
                    break;
                }
            }
        }else{
            foreach (Entity e in owner._entitiesInVision){
                C_Health cHealth = e.GetComponent<C_Health>();
                if (cHealth == null) continue;
                if (e.GetPlayer() != owner.GetPlayer() && owner.GetPlayer()!=null){
                    target = cHealth;
                    break;
                }
            }
        }
            
        if (target != null){
            StartCoroutine(CheckTargetRotation());
            if (owner.Animator != null){
                owner.Animator.SetBool(PulledSword, true);
                if (CWeapon != null) CWeapon.SetWeaponActive(true);
            }
            MoveToTarget();
        }
    }

    private void EntityDied(C_Health cHealth){
        
        if (cHealthsInAttackRange.Contains(cHealth)){
            cHealthsInAttackRange.Remove(cHealth);
            if (target == cHealth){
                ResetTarget();
                SetTarget();
            }
        }
    }
    
    public C_Health GetTarget(){
        return target;
    }

    private bool IsTargetInAttackRange(){
        return CWeapon.GetAttackRange() >Vector3.Distance(owner.transform.position,target.transform.position);
    }
    
    public void DetectorNotification(Component component, Detector.DetectionManagement direction){
        Entity e = component.GetComponent<Entity>();
        if (e.CHealth == null) return;
        if (direction == Detector.DetectionManagement.Enter && !cHealthsInAttackRange.Contains(e.CHealth)){
            cHealthsInAttackRange.Add(e.CHealth);
        }else if (direction == Detector.DetectionManagement.Exit && cHealthsInAttackRange.Contains(e.CHealth)){
            cHealthsInAttackRange.Remove(e.CHealth);
            if (cHealthsInAttackRange.Count == 0 || e.CHealth == target){
                ResetTarget();
            }
        }
        if(target==null){
            SetSpecificTarget(this,e.CHealth);     
        }
        if (e.CHealth == target && direction==Detector.DetectionManagement.Enter){
            StartCoroutine(Attack());
        }
    }
}
