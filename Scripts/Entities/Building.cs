using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : Entity{

    public static readonly int maxCrew = 4;
    [SerializeField] private Transform archerContainer;
    [SerializeField] private Formation formation;
    public Vector3 relativeFormationPos = Vector3.zero;
    public List<Transform> flagPositions = new();
    public GameObject flagPrefab;
    
    
    
        protected override void Start(){
            base.Start();
            
        }


        private void OnTriggerEnter(Collider other){
            Unit triggeredUnit = other.GetComponent<Unit>();
            if (triggeredUnit == null || !triggeredUnit.Crew) return;
            if (triggeredUnit.GetNation() == GetNation()){
                AddUnitToCrew(triggeredUnit); 
            }
        }

        private void AddUnitToCrew(Unit u){
            if (archerContainer.transform.childCount < maxCrew){
                Instantiate(flagPrefab, flagPositions[archerContainer.transform.childCount]);
                u.TowerCrew(this);
                u.transform.SetParent(archerContainer);
                if (cCombat != null && cCombat.GetWeapon()!=null){
                    cCombat.GetWeapon().AttackSpeed /= 2;
                }
            }
        }

        public string GetStats(){
            return "HP: " + cHealth.GetCurrentHp() + " / " + cHealth.GetMaxHp() + "\n" +
                   "CREW: " +   archerContainer.transform.childCount + " / " + maxCrew;
            
        }
 
        
        
        
}
