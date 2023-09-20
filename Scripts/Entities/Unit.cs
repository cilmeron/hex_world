using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;

public class Unit : Entity, IFormationElement, IMoveable{

    #region Fields
        [SerializeField] private Vector3 moveToPosition;
        [SerializeField] private float stoppingDistance = 2f;
        [SerializeField] private Formation formation;
        public Vector3 relativeFormationPos = Vector3.zero;
        [SerializeField] private bool shouldMove = true;
        
        private NavMeshAgent navMeshAgent;
    

    #endregion

    #region Unit Implementation

        public void SetRange(int range){
            vision.UpdateRange(range);
        }
    
        public Vector3 MoveToPosition{
            get => moveToPosition;
            set => moveToPosition = value;
        }
    
    #endregion
    
    #region Selectable Implementation

        protected override void Awake(){
            base.Awake();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        protected override void Start(){
            base.Start();
            navMeshAgent.stoppingDistance = stoppingDistance;
            moveToPosition = transform.position;
            
            if (!shouldMove){
                navMeshAgent.enabled = false;
            }
        }
        
    
        // Update is called once per frame
        protected override void Update(){
            base.Update();
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                if (IsInFormation()){
                    navMeshAgent.stoppingDistance = 0;
                }
                else{
                    navMeshAgent.stoppingDistance = stoppingDistance;
                }
                navMeshAgent.SetDestination(moveToPosition);
            }
        }
        
        public override void OnDeath(){
            base.OnDeath();
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                navMeshAgent.enabled = false;
            }
            if (IsInFormation()){
                formation.RemoveFormationElement(this);
            }
        }
        
        public override void SetMaterialsAndShaders(){
            base.SetMaterialsAndShaders();
            SelectableMas = new SelectableMas(player.PlayerMas.MUnit,player.PlayerMas.MSelectedUnit,player.PlayerMas.MLeaderUnit);
            if (IsSelected()){
                GetComponent<Renderer>().material = SelectableMas.MSelected;
                return;
            }
            if (IsInFormation() && IsLeaderOfFormation()){
                GetComponent<Renderer>().material = SelectableMas.MLeader;
                return;
            }
            GetComponent<Renderer>().material = SelectableMas.MUnselected;
        }

        public override IFormationElement GetFormationElement(){
            return this;
        }
        

    #endregion

    #region IMoveable Implementation

        public void EnableNavMesh(bool active){
            navMeshAgent.enabled = active;
        }
        
        public void SetMoveToPosition(Vector3 moveToPosition){
            this.moveToPosition = moveToPosition;
        }


    #endregion
    
    #region IFormationElement Implementation

    public void RemoveEntityFromFormation(){
        formation = null;
        relativeFormationPos = Vector3.zero;
        SetMaterialsAndShaders();
    }

    

    public void AddEntityToFormation(Formation f,Vector3 relativePos){
        formation = f;
        relativeFormationPos = relativePos;
        SetMaterialsAndShaders();
    }

    public bool IsInFormation(){
        return formation != null;
    }

    public Formation GetFormation(){
        if (IsInFormation()){
            return formation;
        }
        return null;
    }


    private bool IsLeaderOfFormation(){
        return IsInFormation() && formation.GetLeader().GetGameObject() == gameObject;
    }

    public Vector3 GetRelativePosition(){
        return relativeFormationPos;
    }

    public Renderer GetRenderer(){
        return gameObject.GetComponent<Renderer>();
    }

    public ISelectable GetSelectable(){
        return this;
    }

    #endregion
}
