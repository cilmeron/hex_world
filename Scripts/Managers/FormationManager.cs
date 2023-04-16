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
                if (unit.IsInFormation()){
                    return;
                }
                bool successfull = rectFormation.AddUnitToFormation(unit);
                if (!successfull){
                    rectFormation = CreateRectFormation();
                    rectFormation.AddUnitToFormation(unit);
                }
            }
        }
    }
    
    private RectFormation CreateRectFormation(){
        GameObject formation = new GameObject("Formation");
        formation.transform.position = new Vector3(0, 0, 0);
        formation.transform.parent = transform;
        RectFormation rectFormation = formation.AddComponent<RectFormation>();
        rectFormation.Initialize(3);
        return rectFormation;
    }
}
