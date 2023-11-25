using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Combat : MonoBehaviour, DetectorNotification
{
    [SerializeField] private List<C_Health> cHealthsInAttackRange = new List<C_Health>();
    [SerializeField] private C_Health target;
    [SerializeField] public bool userTarget = false;
    private Entity owner;
    [SerializeField] private C_Weapon CWeapon;
    private static readonly int PulledSword = Animator.StringToHash("PulledSword");
    [SerializeField] private float timeTillAttack = 0f;
    [SerializeField] public Detector _attackDistanceDetector;
    private float timeTillTargetRotationCheck = 0f;
    private DecisionTree decisionTree;

    void Awake()
    {
        owner = gameObject.GetComponent<Entity>();
        _attackDistanceDetector.SetOwner(owner);
        decisionTree = new DecisionTree();
    }

    void Start()
    {
        if (CWeapon != null) CWeapon.SetEntity(owner);
        EventManager.Instance.deathEvent.AddListener(EntityDied);
        EventManager.Instance.setTarget.AddListener(SetSpecificUserTarget);
        _attackDistanceDetector.SetDetectorNotification(this);
        _attackDistanceDetector.SetRadius(CWeapon.GetAttackRange());
    }

    private IEnumerator CheckTargetRotation()
    {
        if (target != null)
        {
            timeTillTargetRotationCheck = 3f;
            while (timeTillTargetRotationCheck > 0)
            {
                yield return null; // wait for the next frame
                timeTillTargetRotationCheck -= Time.deltaTime;
            }
            if (target != null)
            {
                StartCoroutine(TurnToTarget());
            }

            StartCoroutine(CheckTargetRotation());
        }
    }
    private IEnumerator TurnToTarget()
    {
        int rotationSpeed = 120;
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

    private IEnumerator Attack()
    {
        if (cHealthsInAttackRange.Contains(target))
        {
            timeTillAttack = CWeapon.GetAttackSpeed();
            if (CWeapon != null)
            {
                CWeapon.Attack(target, owner);
            }
            while (timeTillAttack > 0)
            {
                yield return null; // wait for the next frame
                timeTillAttack -= Time.deltaTime;
            }
            if (target != null)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        if (owner.CMoveable != null)
        {
            owner.CMoveable.SetMoveToTransform(target.transform, false);
        }
    }

    private void SetSpecificUserTarget(C_Combat attacker, C_Health target)
    {
        // TODO: if owner has CMoveable, reset userTarget (CMoveable) and target (CMoveable) (basically, reset all movements)
        if (attacker == this && target.Entity.GetPlayer() != attacker.owner.GetPlayer())
        {
            this.target = target;
            userTarget = true;
            if (owner.Animator != null)
            {
                owner.Animator.SetBool(PulledSword, true);
                if (CWeapon != null) CWeapon.SetWeaponActive(true);
            }
            MoveToTarget();
        }
    }

    private void SetSpecificTarget(C_Combat attacker, C_Health target)
    {
        // TODO: if owner has CMoveable, reset userTarget (CMoveable) and target (CMoveable) (basically, reset all movements)
        if (attacker == this && target.Entity.GetPlayer() != attacker.owner.GetPlayer())
        {
            this.target = target;
            if (owner.Animator != null)
            {
                owner.Animator.SetBool(PulledSword, true);
                if (CWeapon != null) CWeapon.SetWeaponActive(true);
            }
            MoveToTarget();
        }
    }

    public void ResetTarget()
    {
        target = null;
        userTarget = false;
        if (owner.Animator != null)
        {
            owner.Animator.SetBool(PulledSword, false);
            owner.Animator.SetBool(C_Weapon.AnimAttack, false);
            if (CWeapon != null) CWeapon.SetWeaponActive(false);
        }
        StopCoroutine(CheckTargetRotation());
    }
    public void SetTarget()
    {
        // if (target != null) return;
        if (userTarget) return;
        if (owner._entitiesInVision.Count == 0)
        {
            ResetTarget();
            return;
        }

        target = decisionTree.ChooseTarget(owner._entitiesInVision, owner, target);

        if (target == null) return; // in case all entities in vision are from same faction

        StartCoroutine(CheckTargetRotation());
        if (owner.Animator != null)
        {
            owner.Animator.SetBool(PulledSword, true);
            if (CWeapon != null) CWeapon.SetWeaponActive(true);
        }
        MoveToTarget();

    }

    private void EntityDied(C_Health cHealth)
    {
        if (cHealth == owner.CHealth)
        {
            owner.GetPlayer().OwnedEntities.Remove(owner);
        }
        if (owner._entitiesInVision.Contains(cHealth.Entity))
        {
            owner._entitiesInVision.Remove(cHealth.Entity);
        }
        if (cHealthsInAttackRange.Contains(cHealth))
        {
            cHealthsInAttackRange.Remove(cHealth);
            if (target == cHealth)
            {
                ResetTarget();
                SetTarget();
            }
        }
    }

    public C_Health GetTarget()
    {
        return target;
    }

    public int GetAttackRange()
    {
        return CWeapon.GetAttackRange();
    }

    private bool IsTargetInAttackRange()
    {
        return CWeapon.GetAttackRange() > Vector3.Distance(owner.transform.position, target.transform.position);
    }

    public void DetectorNotification(Component component, Detector.DetectionManagement direction)
    {
        Entity e = component.GetComponent<Entity>();
        if (e.CHealth == null) return;
        if (direction == Detector.DetectionManagement.Enter && !cHealthsInAttackRange.Contains(e.CHealth))
        {
            cHealthsInAttackRange.Add(e.CHealth);
        }
        else if (direction == Detector.DetectionManagement.Exit &&
            cHealthsInAttackRange.Contains(e.CHealth) &&
            e.GetPlayer() != owner.GetPlayer())
        {
            cHealthsInAttackRange.Remove(e.CHealth);
            if (cHealthsInAttackRange.Count == 0 || e.CHealth == target)
            {
                ResetTarget();
            }
        }
        if (target == null && e.CHealth.IsAlive())
        {
            SetSpecificTarget(this, e.CHealth);
        }
        if (e.CHealth == target && direction == Detector.DetectionManagement.Enter)
        {
            StartCoroutine(Attack());
        }
    }
}
