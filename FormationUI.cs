using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationUI : MonoBehaviour
{
    [SerializeField] private Formation formation;
    [SerializeField] private float spacing = 50f;
    [SerializeField] private GameObject formationSlotPrefab;
    [SerializeField] private RectTransform formationSlotContainer;

    private List<FormationSlotUI> formationSlots = new List<FormationSlotUI>();
    [SerializeField] private Slider slider;
    private selected_dictionary selected_table;
    
    
    private void Start(){
        selected_table = SelectionManager.Instance.selectedDictionary;
        EventManager.Instance.formationChangedEvent.AddListener(UpdateFormation);
        EventManager.Instance.formationDeletedEvent.AddListener(ResetFormation);
    }

    private void Update(){
        if (formation == null){
            return;
        }
        float hpPercentage = formation.GetOverallHp();
        slider.value = hpPercentage;
        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.red, Color.green, hpPercentage);
    }

    public void ResetFormation(IFormation f){
        ClearFormation();
        if (f.GetFormation() == formation){
            formation = null;
        } 
    }
    
    public void UpdateFormation(IFormation f){
        ClearFormation();
        if (f.GetFormation() == null){
            return;
        }
        formation = f.GetFormation();
        foreach (var kvPair in formation.GetRelativePositions()){
            GenerateAndSetSlot(kvPair);
        }
        GenerateAndSetSlot(new KeyValuePair<Vector3, IFormationElement>(Vector3.zero, formation.GetLeader()));
        CenterUIFormation();
    }

    private void GenerateAndSetSlot(KeyValuePair<Vector3,IFormationElement> kvPair){
        GameObject go = Instantiate(formationSlotPrefab, transform);
        FormationSlotUI slot = go.GetComponent<FormationSlotUI>();
        slot.Formation = formation;
        Vector3 relPos = kvPair.Key;
        slot.RelFormationPos = relPos;
        Vector3 uiPos = new Vector3(relPos.x, relPos.z, 0);
        go.transform.localPosition =  uiPos * spacing;
        go.transform.SetParent(formationSlotContainer, true);
        formationSlots.Add(slot);
        if (kvPair.Value == null){
            slot.image.color = Color.red;
            return;
        }
        slot.FormationElement = kvPair.Value;
        if (kvPair.Value.IsLeader()){
            slot.image.color = Color.green;
        }
    }
    

    public void ClearFormation(){
        foreach (FormationSlotUI slot in formationSlots){
            Destroy(slot.gameObject);
        }
        formationSlots = new List<FormationSlotUI>();
    }

    public void SelectUnitsInFormation(){
        if (formation != null){
            foreach (IFormationElement element in formation.GetFormationElements()){
                if (element.IsSelectable()){
                    selected_table.addSelected(element.GetSelectable());
                }
            }  
        }
    }

    private void CenterUIFormation(){
        RectTransform parentRect = GetComponent<RectTransform>();
        Vector2 parentPos = parentRect.position;
    
        float minX = Mathf.Infinity, maxX = Mathf.NegativeInfinity;
        float minY = Mathf.Infinity, maxY = Mathf.NegativeInfinity;

        foreach (var slot in formationSlots)
        {
            float xPos = slot.transform.position.x;
            float yPos = slot.transform.position.y;
            minX = Mathf.Min(minX, xPos);
            maxX = Mathf.Max(maxX, xPos);
            minY = Mathf.Min(minY, yPos);
            maxY = Mathf.Max(maxY, yPos);
        }

        float xOffset = parentPos.x - ((maxX - minX) / 2 + minX);
        float yOffset = parentPos.y - ((maxY - minY) / 2 + minY);

        foreach (var slot in formationSlots)
        {
            Vector3 pos = slot.transform.position;
            pos.x += xOffset;
            pos.y += yOffset;
            slot.transform.position = pos;
        }
    }
    
    
}