using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITopBar : MonoBehaviour{
    [SerializeField] private List<UITopBarNode> nodes = new List<UITopBarNode>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (UITopBarNode node in nodes){
            node.SetImageColor(GameManager.Instance.player.PlayerColor);
        }
    }

    public List<UITopBarNode> Nodes{
        get => nodes;
        set => nodes = value;
    }
}
