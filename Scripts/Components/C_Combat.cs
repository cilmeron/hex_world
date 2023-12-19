using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class C_Combat : MonoBehaviour
{
    private DecisionTree decisionTree;
    private Entity owner;
    [SerializeField] public bool userTarget = false;
    private static readonly int PulledSword = Animator.StringToHash("PulledSword");
    private float timeTillTargetRotationCheck = 0f;
    
    public List<C_Attack> attacks = new();

    void Awake()
    {
        owner = gameObject.GetComponent<Entity>();
        foreach (C_Attack attack in attacks){
            attack.attackVision.SetOwner(owner);
        }
        decisionTree = new DecisionTree();
    }

    void Start()
    {
        foreach (C_Attack attack in attacks){
            attack.weapon.SetEntity(owner);
            attack.attackVision.SetRadius(attack.weapon.AttackRange);
        }
        EventManager.Instance.deathEvent.AddListener(EntityDied);
        EventManager.Instance.setTarget.AddListener(SetSpecificUserTarget);
        EventManager.Instance.componentDetected.AddListener(ComponentDetection);
    }

    private IEnumerator CheckTargetRotation(C_Attack attack)
    {
        if (owner is not Unit) yield return null;
        if (attack.target != null)
        {
            timeTillTargetRotationCheck = 3f;
            while (timeTillTargetRotationCheck > 0)
            {
                yield return null; // wait for the next frame
                timeTillTargetRotationCheck -= Time.deltaTime;
            }
            if (attack.target != null)
            {
                StartCoroutine(TurnToTarget(attack));
            }

            StartCoroutine(CheckTargetRotation(attack));
        }
    }
    private IEnumerator TurnToTarget(C_Attack attack){
      
        
        int rotationSpeed = 120;
        if (attack.target != null)
        {
            // Calculate the direction to the target
            Vector3 targetDirection = attack.target.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            while (transform.rotation != targetRotation)
            {
                float step = rotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
                yield return null;
            }
        }
    }

    private IEnumerator Attack(C_Attack attack)
    {
        if (attack.VisionContainsTarget())
        {
            attack.timeTillAttack = attack.weapon.AttackSpeed;
            
            attack.weapon.Attack(attack.target, owner);
            
            while (attack.timeTillAttack > 0)
            {
                yield return null; // wait for the next frame
                attack.timeTillAttack -= Time.deltaTime;
            }
            if (attack.target != null)
            {
                StartCoroutine(Attack(attack));
            }
        }
        else
        {
            MoveToTarget(attack);
        }
    }

    private void MoveToTarget(C_Attack attack)
    {
        if (owner.CMoveable != null)
        {
            owner.CMoveable.SetMoveToTransform(attack.target.transform, false);
        }
    }

    private void SetSpecificUserTarget(C_Combat attacker, C_Health target)
    {
        foreach (C_Attack attack in attacks){
            // TODO: if owner has CMoveable, reset userTarget (CMoveable) and target (CMoveable) (basically, reset all movements)
            if (attacker == this && target.Entity.GetNation() != attacker.owner.GetNation())
            {
                attack.target = target;
                userTarget = true;
                if (owner.Animator != null)
                {
                    owner.Animator.SetBool(PulledSword, true);
                    attack.weapon.SetWeaponActive(true);
                }
                MoveToTarget(attack);
            }
        }
    }

    private void SetSpecificTarget(C_Attack attack,C_Combat attacker, C_Health target)
    {
        // TODO: if owner has CMoveable, reset userTarget (CMoveable) and target (CMoveable) (basically, reset all movements)
        if (attacker == this && target.Entity.GetNation() != attacker.owner.GetNation())
        {
            attack.target = target;
            if (owner.Animator != null)
            {
                owner.Animator.SetBool(PulledSword, true);
                attack.weapon.SetWeaponActive(true);
            }
            MoveToTarget(attack);
        }
    }

    public void ResetTarget(C_Attack attack)
    {
        attack.target = null;
        userTarget = false;
        if (owner.Animator != null)
        {
            owner.Animator.SetBool(PulledSword, false);
            owner.Animator.SetBool(C_Weapon.AnimAttack, false);
            attack.weapon.SetWeaponActive(false);
        }
        StopCoroutine(CheckTargetRotation(attack));
    }

    private void SetTarget(C_Attack attack)
    {
        // if (target != null) return;
        if (userTarget) return;
        if (owner.GetEntitiesInVision().Count == 0)
        {
            ResetTarget(attack);
            return;
        }

        attack.target = decisionTree.ChooseTarget(owner.GetEntitiesInVision(), owner, attack);

        if (attack.target == null) return; // in case all entities in vision are from same faction

        StartCoroutine(CheckTargetRotation(attack));
        if (owner.Animator != null)
        {
            owner.Animator.SetBool(PulledSword, true);
            attack.weapon.SetWeaponActive(true);
        }
        MoveToTarget(attack);

    }

    private void EntityDied(C_Health cHealth)
    {
        if (cHealth == owner.CHealth){
            GameManager.Instance.GetPlayerWithNation(owner.GetNation()).OwnedEntities.Remove(owner);
        }
        if (owner.GetEntitiesInVision().Contains(cHealth.Entity))
        {
            owner.RemoveEntityInVision(cHealth.Entity);
        }

        foreach (C_Attack attack in attacks){
            if (GetCHealthsInAttackRange(attack.attackVision).Contains(cHealth))
            {
                GetCHealthsInAttackRange(attack.attackVision).Remove(cHealth);
                if (attack.target == cHealth)
                {
                    ResetTarget(attack);
                    SetTarget(attack);
                }
            }
        }
        
    }
    

    public List<C_Attack> GetAttacks(){
        return attacks;
    }
    
    
    public void ResetAllTargets(){
        foreach (C_Attack attack in attacks){
            ResetTarget(attack);
        }
    }
    
    private void ComponentDetection(Detector detector,Component component, Detector.DetectionManagement direction){

        foreach (C_Attack attack in attacks){
            if (attack.attackVision == detector){
                Entity e = component.GetComponent<Entity>();
                if (e.CHealth == null) return;
                if (attack.attackVision.detectedObjects.Count == 0 || e.CHealth == attack.target)
                {
                    ResetTarget(attack);
                }
                if (attack.target == null && e.CHealth.IsAlive())
                {
                    SetSpecificTarget(attack,this, e.CHealth);
                }
                if (e.CHealth == attack.target && direction == Detector.DetectionManagement.Enter)
                {
                    StartCoroutine(Attack(attack));
                }
            }
                
        }

        
        
        //Entity e = component.GetComponent<Entity>();
        //if (e.CHealth == null) return;
        //if (direction == Detector.DetectionManagement.Enter && !GetCHealthsInAttackRange().Contains(e.CHealth))
        //{
        //    GetCHealthsInAttackRange().Add(e.CHealth);
        //}
        //else if (direction == Detector.DetectionManagement.Exit &&
        //         GetCHealthsInAttackRange().Contains(e.CHealth) &&
        //    e.GetNation() != owner.GetNation())
        //{
        //    GetCHealthsInAttackRange().Remove(e.CHealth);
        //    if (GetCHealthsInAttackRange().Count == 0 || e.CHealth == target)
        //    {
        //        ResetTarget();
        //    }
        //}
        //if (target == null && e.CHealth.IsAlive())
        //{
        //    SetSpecificTarget(this, e.CHealth);
        //}
        //if (e.CHealth == target && direction == Detector.DetectionManagement.Enter)
        //{
        //    StartCoroutine(Attack());
        //}
    }

    public List<C_Health> GetCHealthsInAttackRange(Detector detector){
        return detector.detectedObjects
            .OfType<C_Health>()
            .ToList();
    }
    
    
    public void EnableRangeVisualisation(bool enable){
        foreach (C_Attack attack in attacks){
            attack.attackVision.EnableVisualisation(enable);
        }
    }
}
