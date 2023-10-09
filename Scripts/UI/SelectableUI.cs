using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableUI : MonoBehaviour{

    [SerializeField] private Slider slider;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI statsTMP;
    [SerializeField] private Button openFormationButton;

    private UIManager uiManager;
    void Start()
    {
        uiManager = UIManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        SetValues();
    }

    

    private void SetValues(){
        PrepareSlider();
        image.sprite = uiManager.Selectable.Entity.GetSprite();
        statsTMP.text = uiManager.Selectable.GetStats();
        if (uiManager.Selectable.Entity.CFormation == null){
            openFormationButton.gameObject.SetActive(false);
        }
        else{
            openFormationButton.gameObject.SetActive(uiManager.Selectable.Entity.CFormation.IsInFormation());
        }
        
    }

    public void ResetValues(){
        image.sprite = null;
    }

    private void PrepareSlider(){

        // Calculate the HP percentage (as a value between 0 and 1)
        float hpPercentage = uiManager.Selectable.GetHpPercentage();
        
        slider.value = hpPercentage;
        // Set the color of the slider based on the HP percentage
        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.red, Color.green, hpPercentage);
    }

    
    public void OpenFormationMenu(){
        if (uiManager.Selectable == null){
            return;
        }
        if (uiManager.Selectable.Entity.CFormation==null){
            return;
        }

        C_Formation formationElement = uiManager.Selectable.Entity.CFormation;
        if (!formationElement.IsInFormation()){
            return;
        }

        uiManager.SetFormationUI(formationElement.Formation);
    }
    
}
