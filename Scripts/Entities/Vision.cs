using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour{
    public int radius;
    private SphereCollider visionCollider;
    private Entity e;
    private void Awake(){
        visionCollider = GetComponent<SphereCollider>();
        visionCollider.radius = radius;
        e = transform.parent.gameObject.GetComponent<Entity>();
    }
    
    private void OnTriggerEnter(Collider other){
        Entity triggerEntity = other.gameObject.GetComponent<Entity>();
        if (triggerEntity != null && triggerEntity.GetPlayer() != e.GetPlayer()){
            if (e.Target == null){
                e.Target = triggerEntity;
            }
            e.EntitiesInVision.Add(triggerEntity);
        }
        
    }

    private void OnTriggerExit(Collider other){
        Entity triggerEntity = other.gameObject.GetComponent<Entity>();
        if (triggerEntity != null){
            if (e.Target == triggerEntity){
                e.Target = null;
            }
            e.EntitiesInVision.Remove(triggerEntity);
        }
       
    }

    public void UpdateRange(int range){
        radius = range;
        visionCollider.radius = radius;
    }
    
}
