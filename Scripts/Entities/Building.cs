using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : Entity{

    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] private Transform archerContainer;
    
    
    private void Awake(){
        base.Awake();
    }

    private void Start(){
        base.Start();
        foreach (Transform t in spawnPositions){
            GameObject go = Instantiate(unitPrefab,archerContainer);
            go.transform.localPosition = t.localPosition;
            go.transform.localScale /= transform.localScale.x; // Assumption: Entity is scaled similar in each direction
            Unit unit = go.GetComponent<Unit>();
            unit.SetPlayer(player);
            player.AddEntity(unit);
            unit.SetMaterials();
            unit.SetRange(50);
            unit.EnableNavMesh(false);
        }
    }

    // Update is called once per frame
    private void Update(){
        base.Update();
    }

    public override void SetMaterials(){
        base.SetMaterials();
        material = player.Tower;
        selectedMaterial = player.SelectedTower;
        selectedMaterial.shader = Shader.Find("Custom/S_Outline");
        GetComponent<Renderer>().material = material;
    }

    public override string GetStats(){
        return "HP: " + currentHP + " / " + maxHP + "\n" +
               "CREW: " +   archerContainer.transform.childCount + " / " + spawnPositions.Count;
        
    }
}
