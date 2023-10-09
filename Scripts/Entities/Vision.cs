using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour{
    public int radius;
    private SphereCollider visionCollider;
    [SerializeField]
    private C_Combat cCombat;
    private void Awake(){
        visionCollider = GetComponent<SphereCollider>();
        visionCollider.radius = radius;
    }
    
    private void OnTriggerEnter(Collider other){
        C_Health triggeredCHealth = other.gameObject.GetComponent<C_Health>();
        if (triggeredCHealth != null){
            cCombat.AddCHealthToVision(triggeredCHealth);
            if (cCombat.GetTarget() == null){
                cCombat.SetTarget();
            }
            
        }
    }

    //private MAS Löschen; 
    //Unit + Entity Implementierung Aufräumen;
    
    

    private void OnTriggerExit(Collider other){
        C_Health triggeredCHealth = other.gameObject.GetComponent<C_Health>();
        if (triggeredCHealth != null){
            cCombat.RemoveCHealthFromVision(triggeredCHealth);
            if (cCombat.GetTarget() == null || cCombat.GetTarget() == triggeredCHealth){
                cCombat.SetTarget();
            }
        }
    }

    public void UpdateRange(int range){
        radius = range;
        visionCollider.radius = radius;
    }
    
}
