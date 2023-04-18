using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    [SerializeField] private List<Entity> entities = new List<Entity>();

    [SerializeField] private Color playerColor;
    [SerializeField] public Material unit;
    [SerializeField] public Material selectedUnit;
    [SerializeField] public Material leaderUnit;
    [SerializeField] public Material tower;
    [SerializeField] public Material selectedTower;
    
    void Awake(){
        foreach (Entity e in entities){
            e.SetPlayer(this);
        }
       
        
    }

    void Start(){
        EventManager.Instance.deathEvent.AddListener(RemoveEntity);
        SetupMaterials();
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
    
}
