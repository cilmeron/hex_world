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
    [SerializeField] private UITopBar uiTopbar;
    [SerializeField] private UIEntity uiEntity;
    //[SerializeField] private UIFormation uiFormation;

    [SerializeField] private Entity selectedEntity;
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
        EventManager.Instance.formationDeletedEvent.AddListener(ResetFormation);
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
        if (selectedFormation == null){
            SetEntityUI();
        }
        else{
            SetFormationUI();
        }
        
        SetTopBarUI();
    }

    public void SetFormationUI(){
        uiEntity.gameObject.SetActive(false);
      //  uiFormation.gameObject.SetActive(true);
     //   uiFormation.SetFormation(selectedFormation);
        
    }
    
    public void SetEntityUI(){
        int selectionCount = selectionManager.selectedDictionary.selectedTable.Values.Count;
        if (selectionCount == 0){
            uiEntity.ResetEntityUI();
            uiEntity.gameObject.SetActive(false);
        }else if ( selectionCount == 1){
            selectedEntity = selectionManager.selectedDictionary.selectedTable.Values.First();
            uiEntity.gameObject.SetActive(true);
        //    uiFormation.gameObject.SetActive(false);
            uiEntity.SetEntityUI(selectedEntity);
        }else{
            //Multiple Selection   
        }
    }
    
    public void SetTopBarUI(){
        uiTopbar.Nodes[0].SetText(GameResourceManager.GetResourceAmount(player,GameResourceManager.ResourceType.Gold).ToString());
        uiTopbar.Nodes[1].SetText(player.CalculateUnits().ToString());
        uiTopbar.Nodes[2].SetText(player.CalculateBuildings().ToString());
        uiTopbar.Nodes[3].SetText(player.Formations.Count.ToString());
    }
    
    public Entity SelectedEntity{
        get => selectedEntity;
        set => selectedEntity = value;
    }
    
    public Formation SelectedFormation{
        get => selectedFormation;
        set => selectedFormation = value;
    }

    private void ResetFormation(Formation f){
        selectedFormation = null;
    }
    
}
