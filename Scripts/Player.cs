using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class Player : MonoBehaviour{

    #region Fields
        private List<IOwnership> ownedGameobjects = new List<IOwnership>();
        [SerializeField] private List<Formation> formations = new List<Formation>();
    
        [SerializeField] private Color playerColor;
        private PlayerMAS _playerMas;
    

    #endregion
    
    
    
    

    public void AddFormation(IFormation formation){
        formations.Add(formation.GetFormation());
    }
    
    public void RemoveFormation(IFormation formation){
        if (formations.Contains(formation.GetFormation())){
            formations.Remove(formation.GetFormation());
        }
    }

    #region Player Implementation
        public int CalculateUnits(){
            int unitCount = 0;
            foreach(IOwnership ownedGameobject in ownedGameobjects){
                if (ownedGameobject.IsOfType(typeof(Unit))){
                    unitCount++;
                }
            }
    
            return unitCount;
        }
        
        public int CalculateBuildings(){
            int buildingCount = 0;
            foreach(IOwnership ownedGameobject in ownedGameobjects){
                if (ownedGameobject.IsOfType(typeof(Building))){
                    buildingCount++;
                }
            }
    
            return buildingCount;
        }
        private Color DarkenPlayerColorByPercentage(int percentage){
            
            return Color.Lerp(playerColor, Color.black, percentage/100f);
    
        }
        
        private PlayerMAS SetupMaterialsAndShader(){
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
            return new PlayerMAS(unit, selectedUnit, leaderUnit, tower, selectedTower, standardShader, outlineShader);
        }
    
        void Awake(){
            GameResourceManager.AddPlayer(this);
        }
    
        void Start(){
            _playerMas = SetupMaterialsAndShader();
            EventManager.Instance.deathEvent.AddListener(RemoveOwnership);
            EventManager.Instance.formationDeletedEvent.AddListener(RemoveFormation);
            EventManager.Instance.playerSuccessfullyInitialized.Invoke(this);
        }
    
    #endregion
    
    #region Ownership Implementation
        public void InitializeOwnedGameObject(IOwnership ownedGameObject){
            ownedGameObject.SetMaterialsAndShaders();
            AddOwnership(ownedGameObject);
        }
        public void AddOwnership(IOwnership ownership){
            if (ownership == null){
                return;
            }
            if (ownedGameobjects.Contains(ownership)){
                return;
            }
            ownedGameobjects.Add(ownership);
        }
    
        private void RemoveOwnership(IOwnership ownership){
            if (ownership == null){
                return;
            }
            if (ownedGameobjects.Contains(ownership)){
                ownedGameobjects.Remove(ownership);
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

            public PlayerMAS PlayerMas => _playerMas;

            public List<IOwnership> OwnedGameObjects{
                get => ownedGameobjects;
                set => ownedGameobjects = value;
            }

    #endregion
    
}
