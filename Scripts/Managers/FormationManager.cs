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
            List<Unit> selectedUnits = selectionManager.selectedDictionary.selectedTable.Values.OfType<Unit>().ToList();
            if (selectedUnits.Count == 0){
                return;
            }
            RectFormation rectFormation =  CreateRectFormation();
            foreach (Unit unit in selectedUnits){
                if (unit.IsInFormation() || unit.GetPlayer()!=GameManager.Instance.player){
                    return;
                }
                bool successfull = rectFormation.AddUnitToFormation(unit);
                if (!successfull){
                    rectFormation = CreateRectFormation();
                    rectFormation.AddUnitToFormation(unit);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)){
            List<Unit> selectedUnits = selectionManager.selectedDictionary.selectedTable.Values.OfType<Unit>().ToList();
            if (selectedUnits.Count == 0){
                return;
            }
            CircleFormation circleFormation =  CreateCircleFormation();
            foreach (Unit unit in selectedUnits){
                if (unit.IsInFormation()){
                    return;
                }
                bool successfull = circleFormation.AddUnitToFormation(unit);
                if (!successfull){
                    circleFormation = CreateCircleFormation();
                    circleFormation.AddUnitToFormation(unit);
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
