using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
/*
public class BuildManager : MonoBehaviour{
    [SerializeField] private ObservationTower _observationTower;
    [SerializeField] private LumberjackHut _lumberjackHut;
    [SerializeField] private Storage _storage;
    [SerializeField] private Tilemap map;
    private MapManager _mapManager;
    private ResourceManager _resourceManager;
    [SerializeField] private float fadeSpeed = 5f;
    public List<Building> buildings; // evtl nur eine Liste weil man auf die Vector3Int coordinate eh leicht rückschließen kann

    private Building selectedBuilding = null;
    private Building previewBuilding = null;

    private static Color freeTileColor = new Color(18 / 255f, 87 / 255f, 52 / 255f, 125 / 255f);
    private static Color blockedTileColor = new Color(135 / 255f, 10 / 255f, 48 / 255f, 125 / 255f);
    private HashSet<OverlayTile> selectedTiles;

    void Awake(){
        buildings = new List<Building>();
    }
    
    // Start is called before the first frame update
    private void Start(){
        _mapManager = FindObjectOfType<MapManager>();
        _resourceManager = FindObjectOfType<ResourceManager>();
    }
    
    void Update(){
        if (selectedBuilding != null){
            Ray ray;
            Vector3 mousePos = Input.mousePosition;
            ray = Camera.main.ScreenPointToRay(mousePos);
            map.localBounds.IntersectRay(ray, out float distance);
            Vector3 screenSpace = new Vector3(mousePos.x, mousePos.y, distance);
            Vector3 worldSpace = Camera.main.ScreenToWorldPoint(screenSpace);
            Vector3Int gridPosition = map.WorldToCell(worldSpace);
            if (previewBuilding == null || previewBuilding.GetType()!=selectedBuilding.GetType()){ // maybe do this just with a sprite
                DestroyPreview(false);
                previewBuilding = Instantiate(selectedBuilding, transform,true);
                previewBuilding.UseAsPreview();
                previewBuilding.StartFading();
            }
            previewBuilding.transform.position = map.GetCellCenterWorld(gridPosition);
            HashSet<Vector3Int> buildingtiles = previewBuilding.GetBuildingTiles(gridPosition,true);
            SetOverlayTiles(buildingtiles);
            if(Input.GetMouseButtonDown(0)){
                if (map.HasTile(gridPosition)){
                    if(GameResources.HasResources(selectedBuilding.GetBuildingCost())&& AllSelectedTilesBuildable()){
                        Building building = Instantiate(selectedBuilding, map.GetCellCenterWorld(gridPosition), Quaternion.identity);
                        building.Build();
                        DestroyPreview(true);
                    }
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Keypad2)){
            selectedBuilding = _storage;
        }else if (Input.GetKeyDown(KeyCode.Keypad3)){
            selectedBuilding = _lumberjackHut;
        }else if (Input.GetKeyDown(KeyCode.Keypad4)){
            selectedBuilding = _observationTower;
        }else if (Input.GetKey(KeyCode.Escape)){
            DestroyPreview(true);
        }
    }

    private List<Storage> GetAllStorages(){ // make this Generic - T 
        List<Storage> storages = new List<Storage>();
        if (buildings.Count > 0){
            foreach (var building in buildings){
                if(building.GetType() == typeof(Storage))
                {
                    storages.Add(building as Storage);
                }
            }
        }
        return storages;
    }
    
    [CanBeNull]
    private List<ObservationTower> GetAllObservationTowers(){ // make this Generic - T 
        if (buildings.Count > 0){
            List<ObservationTower> storages = new List<ObservationTower>();
            foreach (var building in buildings){
                if(buildings.GetType() == typeof(ObservationTower))
                {
                    storages.Add(building as ObservationTower);
                }
            }
        }
        return null;
    }
    
    public Storage GetNearestStorage(Unit u){
        Storage nearestStorage = null;
        float minDistance = float.MaxValue;
        foreach (var storage in GetAllStorages()){
            if(!storage.HasFreeStorage()) continue;
            float distance = Vector2.Distance(u.transform.position, storage.transform.position);
            if (distance < minDistance){
                minDistance = distance;
                nearestStorage = storage;
            } 
        }
        return nearestStorage;
    }

    public void AddBuilding(Building building){
        buildings.Add(building);
    }

    public void RemoveBuilding(Building building){
        if (buildings.Contains(building)){
            buildings.Remove(building);
        }
    }

    public Building GetBuildingAt(Vector3Int pos){
        foreach (var building in buildings){
            if (_mapManager.GetTileMap().WorldToCell(building.transform.position) == pos){
                return building;
            }
        }
        return null;
    }

    public float GetFadeSpeed(){
        return fadeSpeed;
    }

    private void SetOverlayTiles(HashSet<Vector3Int> tilePos){
        ResetSelectedTiles();
        selectedTiles = new HashSet<OverlayTile>();
        foreach (var pos in tilePos){
            OverlayTile oT = _mapManager.GetOverlayTileAtPosition(pos);
            if (oT != null){
                selectedTiles.Add(oT);
                oT.selected = true;
                oT.buildAble = _mapManager.IsTileFree(pos) &&  oT.visited;
            }
        }
    }

    public static Color GetFreeColor(){
        return freeTileColor;
    }
    
    public static Color GetBlockedColor(){
        return blockedTileColor;
    }

    private void ResetSelectedTiles(){
        if (selectedTiles != null){
            foreach (var oT in selectedTiles){
                oT.selected = false;
                oT.buildAble = false;
            }
        }
    }

    private void DestroyPreview(bool resetSelectedBuildilng){
        if (previewBuilding != null){
            previewBuilding.StopFading();
            ResetSelectedTiles();
            Destroy(previewBuilding.gameObject);
            if (resetSelectedBuildilng){
                selectedBuilding = null;
            }
        }
    }
    
    private bool AllSelectedTilesBuildable(){
        if (selectedTiles != null){
            foreach (var selectedTile in selectedTiles){
                if (!selectedTile.buildAble) return false;
            }
            return true;
        }
        return false;
    }
}
*/