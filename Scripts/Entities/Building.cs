using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : Entity, IFormationElement{

    [SerializeField] private Vector3 moveToPosition;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] private Transform archerContainer;
    [SerializeField] private Formation formation;
    public Vector3 relativeFormationPos = Vector3.zero;
    
    #region Selectable Implementation

        protected override void Awake(){
            base.Awake();
        }
    
        protected override void Start(){
            base.Start();
            foreach (Transform t in spawnPositions){
                GameObject go = Instantiate(unitPrefab,archerContainer);
                go.transform.localPosition = t.localPosition;
                go.transform.localScale /= transform.localScale.x; // Assumption: Selectable is scaled similar in each direction
                Unit unit = go.GetComponent<Unit>();
                unit.SetPlayer(player);
                player.AddOwnership(unit);
                unit.SetRange(50);
                unit.EnableNavMesh(false);
            }
        }
    
        // Update is called once per frame
        protected override void Update(){
            base.Update();
        }
    
        public override void SetMaterialsAndShaders(){
            base.SetMaterialsAndShaders();
            SelectableMas = new SelectableMas(player.PlayerMas.MTower,player.PlayerMas.MSelectedTower,player.PlayerMas.MLeaderUnit);
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
    
        public override string GetStats(){
            return "HP: " + currentHP + " / " + maxHP + "\n" +
                   "CREW: " +   archerContainer.transform.childCount + " / " + spawnPositions.Count;
            
        }
        
        public override IFormationElement GetFormationElement(){
            return this;
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
    

    public bool IsLeaderOfFormation(){
        return IsInFormation() && formation.GetLeader().GetGameObject() == gameObject;
    }

    public Vector3 GetRelativePosition(){
        return relativeFormationPos;
    }

    public Renderer GetRenderer(){
        return gameObject.GetComponent<Renderer>();
    }

    public void SetMoveToPosition(Vector3 moveToPosition){
        this.moveToPosition = moveToPosition;
    }

    public ISelectable GetSelectable(){
        return this;
    }

    #endregion
    
   
}
