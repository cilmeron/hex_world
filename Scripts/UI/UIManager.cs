using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour{
    private Player player;
    [SerializeField] private UITopBar uiTopbar;
    [SerializeField] private UIEntity uiEntity;

    [SerializeField] private Entity _entity;
    // Start is called before the first frame update
    void Start()
    {
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
        uiTopbar.Nodes[0].SetText(GameResourceManager.GetResourceAmount(player,GameResourceManager.ResourceType.Gold).ToString());
        uiTopbar.Nodes[1].SetText(player.CalculateUnits().ToString());
        uiTopbar.Nodes[2].SetText(player.CalculateBuildings().ToString());
        uiTopbar.Nodes[3].SetText(player.Formations.Count.ToString());
        if (_entity != null){
            uiEntity.SetEntityUI(_entity);
        }
    }
    
    [SerializeField] private CanvasScaler canvasScaler;

   
    
}
