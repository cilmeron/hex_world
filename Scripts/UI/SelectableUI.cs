using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableUI : MonoBehaviour{
    private ISelectable _selectable;

    [SerializeField] private Slider slider;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI statsTMP;

    private UIManager uiManager;
    void Start()
    {
        uiManager = UIManager.Instance;
        EventManager.Instance.deathEvent.AddListener(ResetSelectableUIViaEvent);
    }

    // Update is called once per frame
    void Update()
    {
        if (_selectable != null){
            SetValues();
        }
        else{
            ResetValues();
        }
    }

    public void SetSelectable(ISelectable selectable){
        _selectable = selectable;
    }
    
    public void ResetSelectableUI(){
            _selectable = null;
    }

    public void ResetSelectableUIViaEvent(ICombatElement combatElement){
        if (_selectable == null){
            return;
        }
        if (combatElement.GetGameObject() == _selectable.GetGameObject()){
            ResetSelectableUI();
        }
    }
    

    public void SetValues(){
        PrepareSlider();
        image.sprite = _selectable.GetSprite();
        statsTMP.text = _selectable.GetStats();
    }

    public void ResetValues(){
        image.sprite = null;
    }

    public void PrepareSlider(){

        // Calculate the HP percentage (as a value between 0 and 1)
        float hpPercentage = _selectable.GetHpPercentage();
        
        slider.value = hpPercentage;
        // Set the color of the slider based on the HP percentage
        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.red, Color.green, hpPercentage);
    }

    public void OpenFormationMenu(){
        if (_selectable == null){
            return;
        }
        if (!_selectable.IsFormationElement()){
            return;
        }
        IFormationElement formationElement = _selectable.GetGameObject().GetComponent<IFormationElement>();
        if (!formationElement.IsInFormation()){
            return;
        }

        uiManager.SetFormationUI(formationElement.GetFormation());
    }
    
}
