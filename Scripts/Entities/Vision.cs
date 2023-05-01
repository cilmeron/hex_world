using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour{
    public int radius;
    private SphereCollider visionCollider;
    private ICombatElement combatElement;
    private void Awake(){
        visionCollider = GetComponent<SphereCollider>();
        visionCollider.radius = radius;
        combatElement = transform.parent.gameObject.GetComponent<ICombatElement>();
    }
    
    private void OnTriggerEnter(Collider other){
        ICombatElement triggerCombatElement = other.gameObject.GetComponent<ICombatElement>();
        if (triggerCombatElement != null && triggerCombatElement.GetPlayer() != combatElement.GetPlayer()){
            combatElement.AddCombatElementToVision(triggerCombatElement);
            if (combatElement.GetTarget() == null){
                combatElement.SetTarget();
            }
            
        }
        
    }

    private void OnTriggerExit(Collider other){
        ICombatElement triggerCombatElement = other.gameObject.GetComponent<ICombatElement>();
        if (triggerCombatElement != null){
            combatElement.RemoveCombatElementFromVision(triggerCombatElement);
            if (combatElement.GetTarget() == null){
                combatElement.SetTarget();
            }
        }
    }

    public void UpdateRange(int range){
        radius = range;
        visionCollider.radius = radius;
    }
    
}
