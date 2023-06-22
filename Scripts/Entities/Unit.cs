using UnityEngine;

public class Unit : Entity{
    public void OnDeath(){
            if (CMoveable != null && CMoveable.NavMeshAgent.enabled)
            {
                CMoveable.NavMeshAgent.enabled = false;
            }
            if (CFormation.IsInFormation()){
                CFormation.Formation.RemoveFormationElement(CFormation);
            }
        }
        
        public override void SetMaterialsAndShaders(){
            base.SetMaterialsAndShaders();
            cLook = new C_Look(gameObject.GetComponent<Renderer>(),
                player.PlayerLook.MUnit,player.PlayerLook.MSelectedUnit,player.PlayerLook.MLeaderUnit);
            if (cSelectable!=null && cSelectable.IsSelected()){
                GetComponent<Renderer>().material = cLook.MSelected;
                return;
            }
            if (CFormation.IsInFormation() && IsLeaderOfFormation()){
                GetComponent<Renderer>().material = cLook.MLeader;
                return;
            }
            GetComponent<Renderer>().material = cLook.MUnselected;
        }


    

    private bool IsLeaderOfFormation(){
        return CFormation.IsInFormation() && CFormation.Formation.GetLeader().gameObject == gameObject;
    }

}
