using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : Entity{
    private void Awake(){
        base.Awake();
    }

    private void Start(){
        base.Start();
        material = MaterialManager.Instance.GetMaterial("M_Tower");
        selectedMaterial = MaterialManager.Instance.GetMaterial("M_SelectedTower");
    }

    // Update is called once per frame
    private void Update(){
        base.Update();
    }
}
