using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;
/*
public class ResourceManager : MonoBehaviour{
    [SerializeField] private Transform crateTransform;
    private Dictionary<Vector3Int, ResourceNode> resourceNodes = new Dictionary<Vector3Int, ResourceNode>();
    private BuildManager _buildManager;
    private MapManager _mapManager;

    void Awake(){
        GameResources.Init();
        _buildManager = FindObjectOfType<BuildManager>();
        _mapManager = FindObjectOfType<MapManager>();
    }
    

    [CanBeNull]
    public ResourceNode GetRandomResourceNode(List<ResourceNode> nonAccessibles = null){
        
        if (resourceNodes.Keys.Count > 0){
            if (nonAccessibles == null){
                return resourceNodes.ElementAt(Random.Range(0, resourceNodes.Count)).Value;
            }
            for (int i = 0; i < resourceNodes.Count; i++){
                ResourceNode possibleResourceNode= resourceNodes.ElementAt(Random.Range(i, resourceNodes.Count)).Value;
                if(!nonAccessibles.Contains(possibleResourceNode)){
                    return possibleResourceNode;
                }
            }
        }
        return null;
    }

    [CanBeNull]
    public ResourceNode GetResourceNodeInRange(Vector3Int currentPos, float distance,List<ResourceNode> nonAccessibles){ // tileDistance??
        List<ResourceNode> resourceNodesInRange = new List<ResourceNode>();
        foreach (var key in resourceNodes.Keys){
            if(nonAccessibles.Contains(resourceNodes[key])){
                continue;
            }
            if (Vector3Int.Distance(resourceNodes[key].GetPosition(), currentPos) < distance){
                resourceNodesInRange.Add(resourceNodes[key]);
            }
        }

        if (resourceNodesInRange.Count == 0) return null;
    return resourceNodesInRange[Random.Range(0,resourceNodesInRange.Count)];
    }
    
    

    public void StoreResourceNode(ResourceNode node){
        resourceNodes.Add(node.GetPosition(),node);
        OverlayTile ot = _mapManager.GetOverlayTileAtPosition(node.GetPosition()); //Error because OverlayTiles are not generated during Awake()..
        if (ot != null){
           ot.isBlocked = true; 
        }
    }

    public void RemoveResourceNode(ResourceNode node){
        if (resourceNodes.ContainsKey(node.GetPosition())){
            resourceNodes.Remove(node.GetPosition());
            OverlayTile ot = _mapManager.GetOverlayTileAtPosition(node.GetPosition());
            if (ot != null){
                ot.isBlocked = false; // This leads to a problem if a resource Node is placed on a non walkable sprite in first hand.. 
            }
        }
    }

    [CanBeNull]
    public ResourceNode GetResourceNodeAt(Vector3Int pos){
        if (resourceNodes.ContainsKey(pos)){
            return resourceNodes[pos];
        }
        return null;
    }
    
}
*/