using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour{
    private Player player;
    [SerializeField] private UITopBar topbar;
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
        topbar.Nodes[0].SetText(GameResourceManager.GetResourceAmount(player,GameResourceManager.ResourceType.Gold).ToString());
        topbar.Nodes[1].SetText(player.CalculateUnits().ToString());
        topbar.Nodes[2].SetText(player.CalculateBuildings().ToString());
        topbar.Nodes[3].SetText(player.Formations.Count.ToString());
    }
    
    [SerializeField] private CanvasScaler canvasScaler;

   
    
}
