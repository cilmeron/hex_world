using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    
    private SelectionManager selectionManager;
    
    // Start is called before the first frame update
    void Start()
    {
        selectionManager = SelectionManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            List<ISelectable> selectables = selectionManager.selectedDictionary.selectedTable.Values.ToList();
            if (selectables.Count == 0){
                return;
            }
            RectFormation rectFormation =  CreateRectFormation();
            foreach (ISelectable selectable in selectables){
                if (!selectable.IsFormationElement()){
                    continue;
                }
                IFormationElement formationElement = selectable.GetFormationElement();
                Debug.Assert(formationElement != null, nameof(formationElement) + " != null");
                if (formationElement.IsInFormation()){
                    continue;
                }
                if (formationElement.GetPlayer()!=GameManager.Instance.player){
                    continue;
                }
                bool successfull = rectFormation.AddFormationElement(selectable.GetFormationElement());
                if (!successfull){
                    rectFormation = CreateRectFormation();
                    rectFormation.AddFormationElement(selectable.GetFormationElement());
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)){
            List<ISelectable> selectables = selectionManager.selectedDictionary.selectedTable.Values.ToList();
            if (selectables.Count == 0){
                return;
            }
            CircleFormation circleFormation =  CreateCircleFormation();
            foreach (ISelectable selectable in selectables){
                if (!selectable.IsFormationElement()){
                    continue;
                }
                IFormationElement formationElement = selectable.GetFormationElement();
                Debug.Assert(formationElement != null, nameof(formationElement) + " != null");
                if (formationElement.IsInFormation()){
                    continue;
                }
                if (formationElement.GetPlayer()!=GameManager.Instance.player){
                    continue;
                }
                bool successfull = circleFormation.AddFormationElement(selectable.GetFormationElement());
                if (!successfull){
                    circleFormation = CreateCircleFormation();
                    circleFormation.AddFormationElement(selectable.GetFormationElement());
                }
            }
        }
    }
    
    private RectFormation CreateRectFormation(){
        GameObject formation = new GameObject("RectFormation");
        formation.transform.position = new Vector3(0, 0, 0);
        formation.transform.parent = transform;
        RectFormation rectFormation = formation.AddComponent<RectFormation>();
        rectFormation.Initialize(3);
        return rectFormation;
    }
    
    private CircleFormation CreateCircleFormation(){
        GameObject formation = new GameObject("CircleFormation");
        formation.transform.position = new Vector3(0, 0, 0);
        formation.transform.parent = transform;
        CircleFormation circleFormation = formation.AddComponent<CircleFormation>();
        circleFormation.Initialize(3);
        return circleFormation;
    }
}
