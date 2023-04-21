using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITopBarNode : MonoBehaviour{
    private TextMeshProUGUI tmp;

    private Image image;
    // Start is called before the first frame update
    void Start(){
        tmp = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetText(String content){
        tmp.text = content;
    }

    public void SetImageColor(Color color){
        image.color = color;
    }
    
}
