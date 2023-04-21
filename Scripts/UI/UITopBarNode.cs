using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITopBarNode : MonoBehaviour{
    private TextMeshProUGUI tmp;
    // Start is called before the first frame update
    void Start(){
        tmp = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(String content){
        tmp.text = content;
    }
    
}
