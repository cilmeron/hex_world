using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.Licensing;
using UnityEngine;
using UnityEngine.AI;


public class C_Moveable : MonoBehaviour{

    private Entity entity;
    [SerializeField] private Vector3 moveToPosition;
    [SerializeField] private Transform moveToTransform;
    [SerializeField] private bool shouldMove = true;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private NavMeshAgent navMeshAgent;
    
    

    void Awake(){
        entity = GetComponent<Entity>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent.stoppingDistance = stoppingDistance;
        moveToPosition = transform.position;
            
        if (!shouldMove){
            navMeshAgent.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            if (entity.CFormation.IsInFormation()){
                navMeshAgent.stoppingDistance = 0;
            }
            else{
                navMeshAgent.stoppingDistance = stoppingDistance;
            }
            navMeshAgent.SetDestination(GetDestination());
            entity.detector.GetRangeProjector().UpdateMaterialProperties();
            if (entity.CCombat != null){
                entity.CCombat._attackDistanceDetector.GetRangeProjector().UpdateMaterialProperties();
            }
        }
    }

    private Vector3 GetDestination(){
        if (moveToTransform != null){
            return moveToTransform.position;
        }

        return moveToPosition;
    }
    
    public void SetMoveToTransform(Transform moveToTransform, bool forceMove){
        this.moveToTransform = moveToTransform;
    }
    
    public void SetMoveToPosition(Vector3 moveToPosition,bool forceMove){
        moveToTransform = null;
        C_Formation formation = entity.CFormation;
        if (formation != null){
            if (formation.IsInFormation() && !formation.IsLeader() && !forceMove){
                return;
            }
        }
        this.moveToPosition = moveToPosition;
    }
    
    public void EnableNavMesh(bool active){
        navMeshAgent.enabled = active;
    }

    public NavMeshAgent NavMeshAgent{
        get => navMeshAgent;
    }
}