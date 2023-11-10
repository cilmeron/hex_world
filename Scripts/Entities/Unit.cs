using UnityEngine;

public class Unit : Entity{

    protected override void Update(){
        base.Update();
        if (Animator != null){
            Animator.gameObject.transform.localPosition = Vector3.zero;
            Animator.gameObject.transform.localRotation = Quaternion.identity;
            UpdateAnimatorStats(); 
        }
    }

    private void UpdateAnimatorStats(){
        Animator.SetFloat(AnimHp,cHealth.GetCurrentHp());
        Animator.SetFloat(AnimVelocity, cMoveable.NavMeshAgent.velocity.magnitude);
    }
    
    public void OnDeath(){
        if (CMoveable != null && CMoveable.NavMeshAgent.enabled)
        {
            CMoveable.NavMeshAgent.enabled = false;
        }
        if (CFormation.IsInFormation()){
            CFormation.Formation.RemoveFormationElement(CFormation);
        }
    }
        
    private bool IsLeaderOfFormation(){
        return CFormation.IsInFormation() && CFormation.Formation.GetLeader().gameObject == gameObject;
    }

    public override void DetectorNotification(Component component, Detector.DetectionManagement direction){
        base.DetectorNotification(component, direction);
        if (cCombat != null){
            cCombat.SetTarget();
        }
    }
    

}