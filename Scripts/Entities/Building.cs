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
        material = player.tower;
        selectedMaterial = player.selectedTower;
        selectedMaterial.shader = Shader.Find("Custom/S_Outline");
        GetComponent<Renderer>().material = material;
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
}
