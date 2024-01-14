using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD

public class C_Combat : MonoBehaviour
{
    public ProjectorController projectorController;
    [SerializeField] private int range;
    [SerializeField] private Vision vision;
    [SerializeField] private float attackTime;
    [SerializeField] private int attackDmg;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform bulletStart;
    [SerializeField] private List<C_Health> cHealthsInVision = new List<C_Health>();
    [SerializeField] private C_Health target;
    private Entity entity;
    [SerializeField] private C_Weapon CWeapon;
    private static readonly int PulledSword = Animator.StringToHash("PulledSword");
    private DecisionTree decisionTree;

    void Awake()
    {
        entity = gameObject.GetComponent<Entity>();
=======
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
        foreach (C_Attack attack in attacks)
        {
            attack.attackVision.SetOwner(owner);
        }
        decisionTree = new DecisionTree();
>>>>>>> TowerBehaviour
    }

    void Start()
    {
<<<<<<< HEAD
        if (CWeapon != null) CWeapon.SetEntity(entity);
        vision.UpdateRange(range);
        EventManager.Instance.deathEvent.AddListener(EntityDied);
        EventManager.Instance.setTarget.AddListener(SetSpecificTarget);
        projectorController.circleRadius = range * entity.transform.localScale.x;
        projectorController.circleColor = entity.GetPlayer().PlayerColor;
        projectorController.UpdateMaterialProperties();
        decisionTree = new DecisionTree();
    }

    private IEnumerator Attack()
    {
        MoveToTarget();
        if (entity.Animator != null)
        {
            entity.Animator.SetBool(PulledSword, true);
            if (CWeapon != null) CWeapon.SetWeaponActive(true);
        }
        if (cHealthsInVision.Contains(target))
        {
            float timeTillAttack = attackTime;
            if (CWeapon != null)
            {
                CWeapon.Attack();
            }


            //ShootBullet();
            while (timeTillAttack > 0)
=======
        foreach (C_Attack attack in attacks)
        {
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
>>>>>>> TowerBehaviour
            {
                yield return null; // wait for the next frame
                timeTillTargetRotationCheck -= Time.deltaTime;
            }
            if (attack.target != null)
            {
                StartCoroutine(TurnToTarget(attack));
            }

<<<<<<< HEAD
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

    private void ShootBullet()
    {
        Transform t = transform;
        GameObject projectileObject = Instantiate(projectile, t.position, t.rotation);
        Projectile bullet = projectileObject.GetComponent<Projectile>();
        bullet.StartPos = bulletStart.position;
        bullet.Owner = this;
        bullet.Target = target.Entity;
        bullet.Damage = attackDmg;
    }

    private void MoveToTarget()
    {
        if (entity.CMoveable != null)
        {
            entity.CMoveable.SetMoveToPosition(target.transform.position, false);
        }
    }

    private void SetSpecificTarget(C_Combat attacker, C_Health target)
    {
        if (attacker == this)
        {
            this.target = target;
            if (this.target != null)
            {
                StartCoroutine(Attack());
=======
            StartCoroutine(CheckTargetRotation(attack));
        }
    }
    private IEnumerator TurnToTarget(C_Attack attack)
    {
        int rotationSpeed = 120;
        if (owner is not Unit) yield return null;
        if (owner is Building) rotationSpeed = 0;
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
        if (owner.CMoveable != null && owner is Unit)
        {
            owner.CMoveable.SetMoveToTransform(attack.target.transform, false);
        }
    }

    private void SetSpecificUserTarget(C_Combat attacker, C_Health target)
    {
        foreach (C_Attack attack in attacks)
        {
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
>>>>>>> TowerBehaviour
            }
        }
    }

<<<<<<< HEAD
    public void SetTarget()
    {
        if (cHealthsInVision.Count == 0)
        {
            if (entity.Animator != null)
            {
                entity.Animator.SetBool(PulledSword, false);
                if (CWeapon != null) CWeapon.SetWeaponActive(false);
            }
            target = null;
=======
    private void SetSpecificTarget(C_Attack attack, C_Combat attacker, C_Health target)
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
>>>>>>> TowerBehaviour
        }
        else
        {
            target = decisionTree.ChooseTarget(cHealthsInVision, entity);
         

<<<<<<< HEAD
            if (target != null)
            {
                StartCoroutine(Attack());
            }
        }
    }

    private void EntityDied(C_Health cHealth)
    {
        if (cHealthsInVision.Contains(cHealth))
        {
            RemoveCHealthFromVision(cHealth);
            if (target == cHealth)
            {
                target = null;
                SetTarget();
=======
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
        if (cHealth == owner.CHealth)
        {
            GameManager.Instance.GetPlayerWithNation(owner.GetNation()).OwnedEntities.Remove(owner);
        }
        if (owner.GetEntitiesInVision().Contains(cHealth.Entity))
        {
            owner.RemoveEntityInVision(cHealth.Entity);
        }

        foreach (C_Attack attack in attacks)
        {
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


    public List<C_Attack> GetAttacks()
    {
        return attacks;
    }


    public void ResetAllTargets()
    {
        foreach (C_Attack attack in attacks)
        {
            ResetTarget(attack);
        }
    }

    private void ComponentDetection(Detector detector, Component component, Detector.DetectionManagement direction)
    {

        foreach (C_Attack attack in attacks)
        {
            if (attack.attackVision == detector)
            {
                Entity e = component.GetComponent<Entity>();
                if (e.CHealth == null) return;
                if (attack.attackVision.detectedObjects.Count == 0 || e.CHealth == attack.target)
                {
                    ResetTarget(attack);
                }
                if (attack.target == null && e.CHealth.IsAlive())
                {
                    SetSpecificTarget(attack, this, e.CHealth);
                }
                if (e.CHealth == attack.target && direction == Detector.DetectionManagement.Enter)
                {
                    StartCoroutine(Attack(attack));
                }
>>>>>>> TowerBehaviour
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

    public List<C_Health> GetCHealthsInAttackRange(Detector detector)
    {
        return detector.detectedObjects
            .OfType<C_Health>()
            .ToList();
    }


    public void EnableRangeVisualisation(bool enable)
    {
        foreach (C_Attack attack in attacks)
        {
            attack.attackVision.EnableVisualisation(enable);
        }
    }
<<<<<<< HEAD

    public C_Health GetTarget()
    {
        return target;
    }

    public void AddCHealthToVision(C_Health cHealth)
    {
        cHealthsInVision.Add(cHealth);
    }

    public void RemoveCHealthFromVision(C_Health cHealth)
    {
        cHealthsInVision.Remove(cHealth);
    }

    public int GetAttackDmg()
    {
        return attackDmg;
    }
=======
>>>>>>> TowerBehaviour
}
