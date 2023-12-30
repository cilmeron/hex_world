using UnityEngine;

public class Unit : Entity
{

    public bool crew = false;

    public bool Crew
    {
        get => crew;
        set => crew = value;
    }

    protected override void Update()
    {
        base.Update();
        if (Animator != null)
        {
            Animator.gameObject.transform.localPosition = Vector3.zero;
            Animator.gameObject.transform.localRotation = Quaternion.identity;
            UpdateAnimatorStats();
        }
    }

    private void UpdateAnimatorStats()
    {
        Animator.SetFloat(AnimHp, cHealth.GetCurrentHp());
        float velocity = cMoveable.NavMeshAgent.velocity.magnitude;
        Animator.SetFloat(AnimVelocity, velocity);
        if (velocity > walkThreshhold)
        {
            Animator.SetTrigger(AnimWalk);
        }
    }

    public void OnDeath()
    {
        if (CMoveable != null && CMoveable.NavMeshAgent.enabled)
        {
            CMoveable.NavMeshAgent.enabled = false;
        }
        if (CFormation.IsInFormation())
        {
            CFormation.Formation.RemoveFormationElement(CFormation);
        }
    }

    private bool IsLeaderOfFormation()
    {
        return CFormation.IsInFormation() && CFormation.Formation.GetLeader().gameObject == gameObject;
    }

    public void TowerCrew(Building b)
    {
        gameObject.SetActive(false);
    }

    public void ActivateUnit()
    {
        gameObject.SetActive(true);
    }

}