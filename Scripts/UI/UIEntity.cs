using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEntity : MonoBehaviour{
    [SerializeField] private IEntityUI entityUI;

    private HpSlider entityUIHpSlider;

    private Sprite entityUISprite;

    private String entityUIStats;

    [SerializeField] private Slider slider;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI statsTMP;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (entityUI != null){
            SetValues();
        }
        else{
            ResetValues();
        }
    }

    public void SetEntityUI(IEntityUI eUI){
        entityUI = eUI;
    }

    public void ResetEntityUI(){
        entityUI = null;
        entityUISprite = null;
        entityUIStats =  null;
    }

    public void SetValues(){
        PrepareSlider();
        image.sprite = entityUI.GetSprite();
        statsTMP.text = entityUI.GetStats();
    }

    public void ResetValues(){
        image.sprite = null;
    }

    public void PrepareSlider(){

        // Calculate the HP percentage (as a value between 0 and 1)
        float hpPercentage = entityUI.GetHpPercentage();
        
        slider.value = hpPercentage;
        // Set the color of the slider based on the HP percentage
        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.red, Color.green, hpPercentage);
    }
    
}
