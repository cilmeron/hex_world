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

    private C_Formation formationElement;
    
    private SelectedDictionary selected_table;
    
    // Start is called before the first frame update
    void Start(){
        image = GetComponent<Image>();
        selected_table = SelectionManager.Instance.selectedDictionary;
    }

    
    public void OnDrop(PointerEventData eventData)
    {
        // Get the dragged game object
        GameObject draggedObject = eventData.pointerDrag;

        formationElement = draggedObject.gameObject.GetComponent<C_Formation>();
        if (formationElement == null){
            return;
        }
        if (formationElement.Entity.GetPlayer() != GameManager.Instance.player){
            return;
        }
        if (formationElement.GetType() != typeof(Unit)){
            return;
        }
        if (formationElement.IsInFormation()){
            formationElement.Formation.RemoveFormationElement(formationElement);
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



    public void SetFormationElement(C_Formation element){
        formationElement = element;
        image.sprite = element.Sprite;
    }
    
    public C_Formation FormationElement{
        get => formationElement;
        set => SetFormationElement(value);
    }

    public void HighlightElement(){
        if (formationElement == null || formationElement.Entity.CSelectable == null){
            return;
        }
        C_Selectable selectable = formationElement.gameObject.GetComponent<C_Selectable>();
        selected_table.AddSelected(selectable);
    }

    public Vector3 RelFormationPos{
        get => relFormationPos;
        set => relFormationPos = value;
    }
}
