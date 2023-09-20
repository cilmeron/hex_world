using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class SelectionComponent : MonoBehaviour{
    private ISelectable selectable;
    private IFormationElement formationElement;
    private ICombatElement combatElement;
    private Renderer formationElementRenderer;
    // Start is called before the first frame update
    void Start(){
        
        //TODO Materials are currently stored in IFormationElement. I think its better to store them in ISelectable as the information "selected" and "unselected" are stored here
        selectable = GetComponent<ISelectable>();
        if (selectable == null){
            throw new Exception("Selectable is non existing");
        }
        if (!selectable.IsFormationElement()){
            throw new Exception("Selectable has no FormationElement");
        }

        formationElement = selectable.GetFormationElement();
        if (formationElement == null){
            return;
        }
        formationElementRenderer = formationElement.GetRenderer();
        formationElementRenderer.material = selectable.GetSelectableMas().MSelected;
        if (selectable.IsCombatElement()){
            combatElement = selectable.GetCombatElement();
            Debug.Assert(combatElement != null, nameof(combatElement) + " != null");
            combatElement.SetHpSliderActive(true);
        }
    }

    private void OnDestroy(){
        if (formationElement!=null && formationElement.IsInFormation() && formationElement.IsLeader()){
            GetComponent<Renderer>().material = selectable.GetSelectableMas().MLeader;
        }
        else{
            formationElementRenderer.material = selectable.GetSelectableMas().MUnselected;
        }

        combatElement?.SetHpSliderActive(false);

    }
}