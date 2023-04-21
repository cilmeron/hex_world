using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    [SerializeField] private List<Entity> entities = new List<Entity>();
    [SerializeField] private List<Formation> formations = new List<Formation>();

    [SerializeField] private Color playerColor;
    private Material unit;
    private Material selectedUnit;
    private Material leaderUnit;
    private Material tower;
    private Material selectedTower;
    
    void Awake(){
        GameResourceManager.AddPlayer(this);
        foreach (Entity e in entities){
            e.SetPlayer(this);
        }
    }

    void Start(){
        EventManager.Instance.deathEvent.AddListener(RemoveEntity);
        
        SetupMaterials();
        InitializeEntities();
    }
    
    public List<Entity> Entities{
        get => entities;
        set => entities = value;
    }

    public void AddEntity(Entity e){
        if (entities.Contains(e)){
            return;
        }
        entities.Add(e);
    }

    public void RemoveEntity(Entity e){
        if (entities.Contains(e)){
            entities.Remove(e);
        }
    }

    private Color DarkenPlayerColorByPercentage(int percentage){
        
        return Color.Lerp(playerColor, Color.black, percentage/100f);

    } 

    private void SetupMaterials(){
        unit = new Material(Shader.Find("Standard"));
        unit.CopyPropertiesFromMaterial(MaterialManager.Instance.GetMaterial("M_Unit"));
        selectedUnit = new Material(Shader.Find("Standard"));
        selectedUnit.CopyPropertiesFromMaterial(MaterialManager.Instance.GetMaterial("M_SelectedUnit"));
        leaderUnit = new Material(Shader.Find("Standard"));
        leaderUnit.CopyPropertiesFromMaterial(MaterialManager.Instance.GetMaterial("M_LeaderUnit"));
        tower = new Material(Shader.Find("Standard"));
        tower.CopyPropertiesFromMaterial(MaterialManager.Instance.GetMaterial("M_Tower"));
        selectedTower = new Material(Shader.Find("Standard"));
        selectedTower.CopyPropertiesFromMaterial(MaterialManager.Instance.GetMaterial("M_SelectedTower"));

        unit.color = playerColor;
        selectedUnit.color = playerColor;
        
        leaderUnit.color = DarkenPlayerColorByPercentage(70);
        tower.color = DarkenPlayerColorByPercentage(70);
        selectedTower.color = DarkenPlayerColorByPercentage(70);
        
        //tower.color = playerColor;
        //leaderUnit.color = playerColor;
        //selectedTower.color = playerColor;
    }

    public void InitializeEntities(){
        foreach (Entity e in entities){
            e.SetMaterials();
        }
    }
    
    public Material Unit{
        get => unit;
    }

    public Material SelectedUnit{
        get => selectedUnit;
    }

    public Material LeaderUnit{
        get => leaderUnit;
    }

    public Material Tower{
        get => tower;
    }

    public Material SelectedTower{
        get => selectedTower;
    }

    public void AddFormation(Formation formation){
        formations.Add(formation);
    }
    
    public void RemoveFormation(Formation formation){
        formations.Remove(formation);
    }

    public List<Formation> Formations{
        get => formations;
        set => formations = value;
    }

    public int CalculateUnits(){
        int unitCount = 0;
        foreach(Entity e in entities){
            if (e.GetType() == typeof(Unit)){
                unitCount++;
            }
        }

        return unitCount;
    }
    
    public int CalculateBuildings(){
        int buildingCount = 0;
        foreach(Entity e in entities){
            if (e.GetType() == typeof(Building)){
                buildingCount++;
            }
        }

        return buildingCount;
    }
    
    
}
