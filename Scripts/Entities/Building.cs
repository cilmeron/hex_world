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
        GetComponent<Renderer>().material = material;
        foreach (Transform t in spawnPositions){
            GameObject go = Instantiate(unitPrefab, t.position, Quaternion.identity);
            go.transform.parent = archerContainer;
            Unit unit = go.GetComponent<Unit>();
            unit.SetPlayer(player);
            player.AddEntity(unit);
            unit.SetMaterials();
            unit.EnableNavMesh(enabled);
        }
    }

    // Update is called once per frame
    private void Update(){
        base.Update();
    }
}
