using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : Entity{
    [SerializeField] private Vector3 movePosition;
    [SerializeField] private float stoppingDistance = 2f;
    
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
    }
    

    // Update is called once per frame
    private void Update(){
        base.Update();
        navMeshAgent.destination = movePosition;
    }

    public Vector3 MovePosition{
        get => movePosition;
        set => movePosition = value;
    }
}
