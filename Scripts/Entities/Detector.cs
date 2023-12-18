using System;
using System.Collections.Generic;
using UnityEngine;


public class Detector : MonoBehaviour{
    private SphereCollider col;
    private Entity owner;
    public GameObject visualisation;

    [SerializeField] private string _detactables;
    private DetectorNotification _detectorNotification;
    
    

    
    
    private void OnTriggerEnter(Collider other){
        Type type = Type.GetType(_detactables);
        Component triggeredComponent = other.gameObject.GetComponent(type);
        if (triggeredComponent != null){
            if (triggeredComponent is Detectable){
                try{
                    _detectorNotification.DetectorNotification(triggeredComponent,DetectionManagement.Enter);
                }
                catch (Exception e){
                    Console.WriteLine(e);
                    Debug.Log("Error occurs here");
                    throw;
                } 
            }
        }
    }
    

    private void OnTriggerExit(Collider other){
        Type type = Type.GetType(_detactables);
        Component triggeredComponent = other.gameObject.GetComponent(type);
        if (triggeredComponent != null){
            if (triggeredComponent is Detectable){
                _detectorNotification.DetectorNotification(triggeredComponent,DetectionManagement.Exit);
            }
        }
    }


    public void SetOwner(Entity entity){
        owner = entity;
    }


    
    public enum DetectionManagement{
        Enter,
        Exit
    }

    public void SetDetectorNotification(DetectorNotification notification){
        _detectorNotification = notification;
    }

    public void SetRadius(int r){
        col = GetComponent<SphereCollider>();
        col.radius = r;
        var t = visualisation.transform;
        Vector3 ls = t.localScale;
        t.localScale = new Vector3(r, ls.y, r);
    }

    public void EnableVisualisation(bool b){
        visualisation.SetActive(b);
    }
    
    
}
