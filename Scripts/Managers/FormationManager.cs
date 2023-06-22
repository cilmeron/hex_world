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
    {                               //TODO wegen Performance auf Events umstellen
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            List<C_Selectable> selectables = selectionManager.selectedDictionary.selectedTable.Values.ToList();
            if (selectables.Count == 0){
                return;
            }
            RectFormation rectFormation =  CreateRectFormation();
            foreach (C_Selectable selectable in selectables){
                C_Formation formationable = selectable.gameObject.GetComponent<C_Formation>();
                if (formationable==null){
                    continue;
                }
                Debug.Assert(formationable != null, nameof(formationable) + " != null");
                if (formationable.IsInFormation()){
                    continue;
                }
                if (formationable.Entity.GetPlayer()!=GameManager.Instance.player){
                    continue;
                }
                bool successfull = rectFormation.AddFormationElement(formationable);
                if (!successfull){
                    rectFormation = CreateRectFormation();
                    rectFormation.AddFormationElement(formationable);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)){
            List<C_Selectable> selectables = selectionManager.selectedDictionary.selectedTable.Values.ToList();
            if (selectables.Count == 0){
                return;
            }
            CircleFormation circleFormation =  CreateCircleFormation();
            foreach (C_Selectable selectable in selectables){
                C_Formation formationable = selectable.gameObject.GetComponent<C_Formation>();
                if (formationable==null){
                    continue;
                }
                Debug.Assert(formationable != null, nameof(formationable) + " != null");
                if (formationable.IsInFormation()){
                    continue;
                }
                if (formationable.Entity.GetPlayer()!=GameManager.Instance.player){
                    continue;
                }
                bool successfull = circleFormation.AddFormationElement(formationable);
                if (!successfull){
                    circleFormation = CreateCircleFormation();
                    circleFormation.AddFormationElement(formationable);
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
