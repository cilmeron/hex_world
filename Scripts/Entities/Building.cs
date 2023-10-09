using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : Entity{

    
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] private Transform archerContainer;
    [SerializeField] private Formation formation;
    public Vector3 relativeFormationPos = Vector3.zero;

    
    
    
        protected override void Start(){
            base.Start();
            foreach (Transform t in spawnPositions){
                GameObject go = Instantiate(unitPrefab,archerContainer);
                go.transform.localPosition = t.localPosition;
                go.transform.localScale /= transform.localScale.x; // Assumption: Selectable is scaled similar in each direction
                Unit unit = go.GetComponent<Unit>();
                unit.SetPlayer(player);
                player.AddOwnership(unit);
                unit.CMoveable.EnableNavMesh(false);
            }
        }
    
        //Spawnpoints ausstellen, dafür bestatzung einfügen und Schaden anpassen. Außerdem zweiten Angriff implementieren
        //Außerdem einfügen von C_Weapon und C_Projectile
        //Außerdem implementieren von weiteren Units
        //Außerdem Preisberechung
            
      
    
        public string GetStats(){
            return "HP: " + cHealth.GetCurrentHp() + " / " + cHealth.GetMaxHp() + "\n" +
                   "CREW: " +   archerContainer.transform.childCount + " / " + spawnPositions.Count;
            
        }
 
        
        
        
}
