using System;
using System.Collections.Generic;
using UnityEngine;


public class Detector : MonoBehaviour{
    public int radius;
    private SphereCollider collider;
    private Entity owner;

    [SerializeField] private ProjectorController projector;
    
    [SerializeField] private string _detactables;
    private DetectorNotification _detectorNotification;
    
    private void Awake(){
        collider = GetComponent<SphereCollider>();
        collider.radius = radius;
    }

    private void Start(){
        UpdateProjector();
    }
    
    private void OnTriggerEnter(Collider other){
        Type type = Type.GetType(_detactables);
        Component triggeredComponent = other.gameObject.GetComponent(type);
        if (triggeredComponent != null){
            if (triggeredComponent is Detectable){
                _detectorNotification.DetectorNotification(triggeredComponent,DetectionManagement.Enter);
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


    public void EnableProjector(bool enable){
        projector.gameObject.SetActive(enable);
    }

    public ProjectorController GetRangeProjector(){
        return projector;
    }
    
    public enum DetectionManagement{
        Enter,
        Exit
    }

    public void SetDetectorNotification(DetectorNotification notification){
        _detectorNotification = notification;
    }

    public void SetRadius(int r){
        radius = r;
        collider.radius = r;
        UpdateProjector();
    }
    
    private void UpdateProjector(){
        projector.circleRadius = radius * owner.transform.localScale.x;
        projector.circleColor = owner.GetPlayer().PlayerColor;
        projector.UpdateMaterialProperties();
        projector.gameObject.SetActive(false);
    }
    
}
