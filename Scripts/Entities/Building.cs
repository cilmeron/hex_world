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

    
    
    
        protected override void Start(){
            base.Start();
            
        }

        public string GetStats(){
            return "HP: " + cHealth.GetCurrentHp() + " / " + cHealth.GetMaxHp() + "\n" +
                   "CREW: " +   archerContainer.transform.childCount + " / " + maxCrew;
            
        }
 
        
        
        
}
