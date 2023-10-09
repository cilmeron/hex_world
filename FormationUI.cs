using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationUI : MonoBehaviour
{
    [SerializeField] private Formation formation;
    [SerializeField] private float spacing = 50f;
    [SerializeField] private GameObject formationSlotPrefab;
    [SerializeField] private RectTransform formationSlotContainer;

    private List<FormationSlotUI> _formationSlots = new List<FormationSlotUI>();
    [SerializeField] private Slider slider;
    private SelectedDictionary _selectedTable;
    
    
    private void Start(){
        _selectedTable = SelectionManager.Instance.selectedDictionary;
        EventManager.Instance.formationChangedEvent.AddListener(UpdateFormation);
        EventManager.Instance.damageEvent.AddListener(UpdateFormationHpSlider);
    }
    
    private void UpdateFormationHpSlider(C_Health health,int damage){
        Entity e = health.Entity;
        C_Formation formationElement = e.CFormation;
        if (e == null || formationElement == null || formation==null){
            return;
        }
        if(formation.GetFormationElements().Contains(formationElement)){
            float hpPercentage = formation.GetOverallHp();
            slider.value = hpPercentage;
            slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.red, Color.green, hpPercentage);
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
        GenerateAndSetSlot(new KeyValuePair<Vector3, C_Formation>(Vector3.zero, formation.GetLeader()));
        CenterUIFormation();
    }

    private void GenerateAndSetSlot(KeyValuePair<Vector3,C_Formation> kvPair){
        GameObject go = Instantiate(formationSlotPrefab, transform);
        FormationSlotUI slot = go.GetComponent<FormationSlotUI>();
        slot.Formation = formation;
        Vector3 relPos = kvPair.Key;
        slot.RelFormationPos = relPos;
        Vector3 uiPos = new Vector3(relPos.x, relPos.z, 0);
        go.transform.localPosition =  uiPos * spacing;
        go.transform.SetParent(formationSlotContainer, true);
        _formationSlots.Add(slot);
        if (kvPair.Value == null){
            slot.image.color = Color.red;
            return;
        }
        slot.FormationElement = kvPair.Value;
        if (kvPair.Value.IsLeader()){
            slot.image.color = Color.green;
        }

        var imageColor = slot.image.color;
        imageColor.a = 150f/255;
        slot.image.sprite = kvPair.Value.Entity.GetSprite();
    }
    

    public void ClearFormation(){
        foreach (FormationSlotUI slot in _formationSlots){
            Destroy(slot.gameObject);
        }
        _formationSlots = new List<FormationSlotUI>();
    }

    public void SelectUnitsInFormation(){
        if (formation != null){
            foreach (C_Formation element in formation.GetFormationElements()){
                if (element.Entity.CSelectable!=null){
                    _selectedTable.AddSelected(element.Entity.CSelectable);
                }
            }  
        }
    }

    private void CenterUIFormation(){
        RectTransform parentRect = GetComponent<RectTransform>();
        Vector2 parentPos = parentRect.position;
    
        float minX = Mathf.Infinity, maxX = Mathf.NegativeInfinity;
        float minY = Mathf.Infinity, maxY = Mathf.NegativeInfinity;

        foreach (var slot in _formationSlots)
        {
            var position = slot.transform.position;
            float xPos = position.x;
            float yPos = position.y;
            minX = Mathf.Min(minX, xPos);
            maxX = Mathf.Max(maxX, xPos);
            minY = Mathf.Min(minY, yPos);
            maxY = Mathf.Max(maxY, yPos);
        }

        float xOffset = parentPos.x - ((maxX - minX) / 2 + minX);
        float yOffset = parentPos.y - ((maxY - minY) / 2 + minY);

        foreach (var slot in _formationSlots)
        {
            var t = slot.transform;
            Vector3 pos = t.position;
            pos.x += xOffset;
            pos.y += yOffset;
            t.position = pos;
        }
    }
    
    
}