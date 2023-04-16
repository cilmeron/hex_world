using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : Entity{
    [SerializeField] private Vector3 movePosition;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private Formation formation;
    public Material leaderMaterial;
    public Vector3 relativeFormationPos = Vector3.zero;
    
    private NavMeshAgent navMeshAgent;

    private void Awake(){
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start(){
        base.Start();
        navMeshAgent.stoppingDistance = stoppingDistance;
        material = MaterialManager.Instance.GetMaterial("M_Unit");
        selectedMaterial = MaterialManager.Instance.GetMaterial("M_SelectedUnit");
        leaderMaterial = MaterialManager.Instance.GetMaterial("M_LeaderUnit");
        movePosition = transform.position;
    }
    

    // Update is called once per frame
    private void Update(){
        base.Update();
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            if (IsInFormation()){
                navMeshAgent.stoppingDistance = 0;
            }
            else{
                navMeshAgent.stoppingDistance = stoppingDistance;
            }
            navMeshAgent.SetDestination(movePosition);
        }
    }

    public Vector3 MovePosition{
        get => movePosition;
        set => movePosition = value;
    }

    protected override void OnDeath(){
        base.OnDeath();
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.enabled = false;
        }
    }

    public void AddUnitToFormation(Formation f,Vector3 relativePos){
        formation = f;
        relativeFormationPos = relativePos;
    }
    
    public void RemoveUnitFromFormation(){
        formation = null;
        relativeFormationPos = Vector3.zero;
    }

    public bool IsInFormation(){
        return formation != null;
    }
    
}
