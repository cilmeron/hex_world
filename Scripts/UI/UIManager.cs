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

    [SerializeField] private ISelectable selectable;
    [SerializeField] private Formation selectedFormation;

    [SerializeField] private CanvasScaler canvasScaler;
    
    private SelectionManager selectionManager;

    // Awake is called when the script instance is being loaded
    private void Awake(){
        if (Instance == null){
            Instance = this;
        } else if (Instance != this){
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start(){
        ScaleUI();
        selectionManager = SelectionManager.Instance;
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
        player = GameManager.Instance.player;
        SetTopBarUI();
        
        if (selectedFormation == null){
            SetSelectableUI();
        }

    }

    public void SetFormationUI(Formation formation){
        selectedFormation = formation;
        selectableUI.gameObject.SetActive(false);
        formationUI.gameObject.SetActive(true);
        formationUI.UpdateFormation(selectedFormation);
        
    }
    
    public void SetSelectableUI(){
        int selectionCount = selectionManager.selectedDictionary.selectedTable.Values.Count;
        if (selectionCount == 0){
            selectableUI.ResetSelectableUI();
            selectableUI.gameObject.SetActive(false);
        }else if ( selectionCount == 1){
            selectable = selectionManager.selectedDictionary.selectedTable.Values.First();
            selectableUI.gameObject.SetActive(true);
            formationUI.gameObject.SetActive(false);
            selectableUI.SetSelectable(selectable);
        }else{
            //Multiple Selection   
        }
    }
    
    public void SetTopBarUI(){
        topBarUITopbar.Nodes[0].SetText(GameResourceManager.GetResourceAmount(player,GameResourceManager.ResourceType.Gold).ToString());
        topBarUITopbar.Nodes[1].SetText(player.CalculateUnits().ToString());
        topBarUITopbar.Nodes[2].SetText(player.CalculateBuildings().ToString());
        topBarUITopbar.Nodes[3].SetText(player.Formations.Count.ToString());
    }
    
    public ISelectable Selectable{
        get => selectable;
        set => selectable = value;
    }
    
    public Formation SelectedFormation{
        get => selectedFormation;
        set => selectedFormation = value;
    }

    public void ResetFormation(){
        if (selectedFormation != null){
            formationUI.ClearFormation();
            selectedFormation = null;
            formationUI.gameObject.SetActive(false);
        }
    }
    
    private void ResetFormationThroughEvent(IFormation f){//Event
        if (f.GetFormation() == selectedFormation || selectedFormation == null){
            selectedFormation = null;
            formationUI.gameObject.SetActive(false);
        }
    }
    
}
