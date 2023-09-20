using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FormationSlotUI : MonoBehaviour, IDropHandler{
    public Image image;

    private Formation formation;
    private Vector3 relFormationPos;

    private IFormationElement formationElement;
    
    private selected_dictionary selected_table;
    
    // Start is called before the first frame update
    void Start(){
        image = GetComponent<Image>();
        selected_table = SelectionManager.Instance.selectedDictionary;
        EventManager.Instance.deathEvent.AddListener(CombatElementDied);
        
    }

    
    public void OnDrop(PointerEventData eventData)
    {
        // Get the dragged game object
        GameObject draggedObject = eventData.pointerDrag;

        formationElement = draggedObject.gameObject.GetComponent<IFormationElement>();
        if (formationElement == null){
            return;
        }
        if (formationElement.GetPlayer() != GameManager.Instance.player){
            return;
        }
        if (formationElement.GetType() != typeof(Unit)){
            return;
        }
        if (formationElement.IsInFormation()){
            formationElement.GetFormation().RemoveFormationElement(formationElement);
        }
        if (!formation.AddFormationElementAt(formationElement,relFormationPos)){
            Debug.Log("Unit was not added to Formation");
            return;
        }
        SetFormationElement(formationElement);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public Formation Formation{
        get => formation;
        set => formation = value;
    }

    private void CombatElementDied(ICombatElement combatElement){
        if (combatElement.GetType() == typeof(IFormationElement) && formationElement!=null){
            IFormationElement tempFormationElement = (IFormationElement) combatElement;
            if (formationElement == tempFormationElement){
                formationElement = null;
            }
        }
    }


    public void SetFormationElement(IFormationElement element){
        formationElement = element;
        image.sprite = element.GetSprite();
    }
    
    public IFormationElement FormationElement{
        get => formationElement;
        set => SetFormationElement(value);
    }

    public void HighlightElement(){
        if (formationElement == null || !formationElement.IsSelectable()){
            return;
        }
        selected_table.addSelected(formationElement.GetSelectable());
    }

    public Vector3 RelFormationPos{
        get => relFormationPos;
        set => relFormationPos = value;
    }
}
