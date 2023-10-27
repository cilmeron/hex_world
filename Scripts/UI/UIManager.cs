using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour{
    // Static instance variable
    public static UIManager Instance { get; private set; }

    // Private constructor
    private UIManager() { }

     private Player player;
    [SerializeField] private TopBarUI topBarUITopbar;
    [SerializeField] private SelectableUI selectableUI;
    [SerializeField] private FormationUI formationUI;

    [SerializeField] private C_Selectable selection;
    [SerializeField] private Formation selectedFormation;

    [SerializeField] private CanvasScaler canvasScaler;
    
    private SelectionManager selectionManager;

    // Awake is called when the script instance is being loaded
    private void Awake(){
        // Check if instance already exists
        if (Instance == null){
            // If not, set instance to this
            Instance = this;
        } else if (Instance != this){
            // If instance already exists and it's not this, destroy this
            Destroy(gameObject);
            return;
        }

        // Set this object to not be destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start(){
        ScaleUI();
        selectionManager = SelectionManager.Instance;
        EventManager.Instance.deathEvent.AddListener(ResetSelectionThroughEvent);
        EventManager.Instance.formationDeletedEvent.AddListener(ResetFormationThroughEvent);
    }

    private void ScaleUI(){
        // Set the reference resolution
        canvasScaler.referenceResolution = new Vector2(1080f, 1920f);

        // Set the screen match mode
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        // Set the match mode value
        canvasScaler.matchWidthOrHeight = 0.5f;
    }

    // Update is called once per frame
    void Update(){ 
        player = GameManager.Instance.player;   //TODO wegen Performance auf Events umstellen
        SetTopBarUI();                          //TODO wegen Performance auf Events umstellen
        SetSelectableUI();   
    }

    
    
    
    #region Topbar
        private void SetTopBarUI(){
            topBarUITopbar.Nodes[0].SetText(GameResourceManager.GetResourceAmount(player,GameResourceManager.ResourceType.Gold).ToString());
            topBarUITopbar.Nodes[1].SetText(player.CalculateUnits().ToString());
            topBarUITopbar.Nodes[2].SetText(player.CalculateBuildings().ToString());
            topBarUITopbar.Nodes[3].SetText(player.Formations.Count.ToString());
        }
    #endregion
    
    #region Selection

    public C_Selectable Selectable{
        get => selection;
        set => selection = value;
    }
    
    private void SetSelectableUI(){
        int selectionCount = selectionManager.selectedDictionary.selectedTable.Values.Count;
        if (selectionCount == 0){
            ResetSelection();
            ResetFormation();
        }else if (selectionCount == 1){
            if (selectedFormation == null){
                ResetFormation();            
                selection = selectionManager.selectedDictionary.selectedTable.Values.First();
                selectableUI.gameObject.SetActive(true);
            }
        }else{
            //Multiple Selection   
        }
    }


    private void ResetSelection(){
        selection = null;
        selectableUI.ResetValues();
        selectableUI.gameObject.SetActive(false);
        
    }
    
    private void ResetSelectionThroughEvent(C_Health health){
        C_Selectable selectable = health.Entity.CSelectable;
        if (selectable != null && selection == selectable){
            ResetSelection();
        }
    }

    #endregion
    
    #region Formation
        public Formation SelectedFormation{
            get => selectedFormation;
            set => selectedFormation = value;
        }    
        public void SetFormationUI(Formation formation){
            selectedFormation = formation;
            selectableUI.gameObject.SetActive(false);
            formationUI.gameObject.SetActive(true);
            formationUI.UpdateFormation(selectedFormation);
            
        }
        
        private void ResetFormation(){
            formationUI.ClearFormation();
            selectedFormation = null;
            formationUI.gameObject.SetActive(false);
        }
        
        private void ResetFormationThroughEvent(IFormation f){//Event
            if (f.GetFormation() == selectedFormation || selectedFormation == null){
                ResetFormation();
            }
        }
    #endregion
}
