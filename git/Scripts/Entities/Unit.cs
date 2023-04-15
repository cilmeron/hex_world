using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : Entity{
    [SerializeField] private Transform movePositionTransform;
    [SerializeField] private float stoppingDistance = 2f;
    
    private NavMeshAgent navMeshAgent;

    private void Awake(){
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start(){
        base.Start();
        navMeshAgent.destination = movePositionTransform.position;
        navMeshAgent.stoppingDistance = stoppingDistance;
    }
    

    // Update is called once per frame
    private void Update(){
        base.Update();
        
    }
    
}
