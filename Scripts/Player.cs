using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class Player : MonoBehaviour{

        private List<Entity> ownedEntities = new List<Entity>();
        [SerializeField] private List<Formation> formations = new List<Formation>();
    
        [SerializeField] private Color playerColor;
        private Player_Look _playerLook;

        [SerializeField]
        public Nations nation;


        public enum Nations{
            VIKINGS,
            SAMURAI
        }





        public void AddFormation(IFormation formation){
        formations.Add(formation.GetFormation());
    }
    
    public void RemoveFormation(IFormation formation){
        if (formations.Contains(formation.GetFormation())){
            formations.Remove(formation.GetFormation());
        }
    }
        public int CalculateUnits(){
            int unitCount = 0;
            foreach(Entity entity in ownedEntities){
                if (entity.GetType() == typeof(Unit)){
                    unitCount++;
                }
            }
    
            return unitCount;
        }
        
        public int CalculateBuildings(){
            int buildingCount = 0;
            foreach(Entity entity in ownedEntities){
                if (entity.GetType() == typeof(Building)){
                    buildingCount++;
                }
            }
    
            return buildingCount;
        }
        private Color DarkenPlayerColorByPercentage(int percentage){
            
            return Color.Lerp(playerColor, Color.black, percentage/100f);
    
        }
        
        private Player_Look SetupMaterialsAndShader(){
            Shader outlineShader = Shader.Find("Custom/S_Outline");
            Shader standardShader = Shader.Find("Standard");
            Material unit = new Material(standardShader);
            unit.CopyPropertiesFromMaterial(MaterialManager.Instance.GetMaterial("M_Unit"));
            Material selectedUnit = new Material(outlineShader);
            selectedUnit.CopyPropertiesFromMaterial(MaterialManager.Instance.GetMaterial("M_SelectedUnit"));
            Material leaderUnit = new Material(standardShader);
            leaderUnit.CopyPropertiesFromMaterial(MaterialManager.Instance.GetMaterial("M_LeaderUnit"));
            Material tower = new Material(standardShader);
            tower.CopyPropertiesFromMaterial(MaterialManager.Instance.GetMaterial("M_Tower"));
            Material selectedTower = new Material(standardShader);
            selectedTower.CopyPropertiesFromMaterial(MaterialManager.Instance.GetMaterial("M_SelectedTower"));
    
            unit.color = playerColor;
            selectedUnit.color = playerColor;
            
            leaderUnit.color = DarkenPlayerColorByPercentage(70);
            tower.color = DarkenPlayerColorByPercentage(70);
            selectedTower.color = DarkenPlayerColorByPercentage(70);
            return new Player_Look(unit, selectedUnit, leaderUnit, tower, selectedTower, standardShader, outlineShader);
        }
    
        void Awake(){
            GameResourceManager.AddPlayer(this);
        }
    
        void Start(){
            _playerLook = SetupMaterialsAndShader();
            //EventManager.Instance.deathEvent.AddListener(RemoveOwnership);
            EventManager.Instance.formationDeletedEvent.AddListener(RemoveFormation);
            EventManager.Instance.playerSuccessfullyInitialized.Invoke(this);
        }
    
    
    #region Ownership Implementation
        public void InitializeOwnedGameObject(Entity entity){
            entity.SetMaterialsAndShaders();
            AddOwnership(entity);
        }
        public void AddOwnership(Entity entity){
            if (entity == null){
                return;
            }
            if (ownedEntities.Contains(entity)){
                return;
            }
            ownedEntities.Add(entity);
        }
    
        private void RemoveOwnership(Entity entity){
            if (entity == null){
                return;
            }
            if (ownedEntities.Contains(entity)){
                ownedEntities.Remove(entity);
            }
        }

    #endregion

    #region Properties
        public List<Formation> Formations{
                get => formations;
                set => formations = value;
            }

            public Color PlayerColor{
                get => playerColor;
                set => playerColor = value;
            }

            public Player_Look PlayerLook => _playerLook;

            public List<Entity> OwnedGameObjects{
                get => ownedEntities;
                set => ownedEntities = value;
            }

    #endregion
    
}
