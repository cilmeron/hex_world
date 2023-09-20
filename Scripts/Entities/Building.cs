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
            
        public override void SetMaterialsAndShaders(){
            base.SetMaterialsAndShaders();
            cLook = new C_Look(gameObject.GetComponent<Renderer>(),player.PlayerLook.MTower,player.PlayerLook.MSelectedTower,player.PlayerLook.MLeaderUnit);
            if (cSelectable!=null && cSelectable.IsSelected()){
                GetComponent<Renderer>().material = cLook.MSelected;
                return;
            }
            if (CFormation!=null &&  CFormation.IsInFormation() && CFormation.IsLeader()){
                GetComponent<Renderer>().material = cLook.MLeader;
                return;
            }
            GetComponent<Renderer>().material = cLook.MUnselected;
        }
    
        public string GetStats(){
            return "HP: " + cHealth.GetCurrentHp() + " / " + cHealth.GetMaxHp() + "\n" +
                   "CREW: " +   archerContainer.transform.childCount + " / " + spawnPositions.Count;
            
        }
 
        
        
        
}
