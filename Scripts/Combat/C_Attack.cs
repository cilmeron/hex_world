using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class C_Attack
{
    public C_Health target;
    public float timeTillAttack;
    public C_Weapon weapon;
    public Detector attackVision;
    

    public bool VisionContainsTarget(){
        return  attackVision.detectedObjects
            .OfType<C_Health>()
            .ToList().Contains(target);
    }
    
    
}