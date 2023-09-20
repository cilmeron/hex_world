using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopBarUI : MonoBehaviour{
    [SerializeField] private List<TopBarUINode> nodes = new List<TopBarUINode>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (TopBarUINode node in nodes){
            node.SetImageColor(GameManager.Instance.player.PlayerColor);
        }
    }

    public List<TopBarUINode> Nodes{
        get => nodes;
        set => nodes = value;
    }
}
