using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;
using Debug = System.Diagnostics.Debug;

public class Formation : MonoBehaviour, IFormation
{
    //TODO Alle Entities sollen wieder anklickbar sein, derzeit funktionieren nur türme
    //TODO Einfügen von Schwerkämpfern
    
    #region Fields
        protected IFormationElement leader;
        protected Dictionary<Vector3,IFormationElement> relativePositions = new Dictionary<Vector3, IFormationElement>();
        protected HashSet<IFormationElement> formationElements = new HashSet<IFormationElement>();
        protected Vector3 targetPos;
        protected int spacingBetweenUnits;
        [SerializeField] protected int maxCapacity;
        #endregion

    #region Formation
    protected virtual void Awake(){
        EventManager.Instance.deathEvent.AddListener(OnEntityDeath);
        GameManager.Instance.player.AddFormation(this);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (formationElements.Count == 0){
            EventManager.Instance.formationDeletedEvent.Invoke(this);
            Destroy(gameObject);
            return;
        }

        UpdateUnitDestinations();
    }
    public void Initialize(int spacing){
        SetSpacing(spacing);
        CalculateRelativePositions();
    }

    #endregion

    #region IFormation
 public void SetLeader(IFormationElement formationElement)
    {
        leader = formationElement;
        if (leader.IsSelectable()){
            leader.GetRenderer().material = leader.GetSelectable().GetSelectableMas().MLeader;
        }
        formationElements.Add(formationElement);
        formationElement.AddEntityToFormation(this,Vector3.zero);
        InvokeFormationChangedEvent();
    }

    public IFormationElement GetLeader()
    {
        return leader;
    }

    public Dictionary<Vector3,IFormationElement> GetRelativePositions(){
        return relativePositions;
    }
    
    public List<IFormationElement> GetFormationElements(){
        return formationElements.ToList();
    }

    public void SetFormation()
    {
        throw new System.NotImplementedException();
    }

    public void SetSpacing(int spacing)
    {
        spacingBetweenUnits = spacing;
    }

    public void SetTargetPosition(Vector3 pos)
    {
        targetPos = pos;
    }

    public void SetFormationElements(List<IFormationElement> formationElements)
    {
        //this.units = units;
        foreach (IFormationElement element in formationElements){
            AddFormationElement(element);
        }
    }

    public bool AddFormationElement(IFormationElement u)
    {
        if (formationElements.Count == maxCapacity)
        {
            return false;
        }

        if (formationElements.Count == 0)
        {
            SetLeader(u);
            return true;
        }

        // Find the first available relative position
        Vector3 relativePosition = Vector3.zero;
        bool nonAllocatedRelativePositionFound = false;
        foreach (Vector3 pos in relativePositions.Keys)
        {
            if (relativePositions[pos] == null)
            {
                relativePosition = pos;
                nonAllocatedRelativePositionFound = true;
                break;
            }
        }

        if (!nonAllocatedRelativePositionFound){
            return false;
        }

        return AddFormationElementAt(u, relativePosition);
    }

    public bool AddFormationElementAt(IFormationElement u, Vector3 relPos){
        // Add the unit to the dictionary at the relative position
        if (!relativePositions.Keys.Contains(relPos)){
            return false;
        }

        if (relativePositions[relPos] != null){
            IFormationElement unitToRemove = relativePositions[relPos];
            this.RemoveFormationElement(unitToRemove);
        }
        
        relativePositions[relPos] = u;
        formationElements.Add(u);
        u.AddEntityToFormation(this,relPos);
        InvokeFormationChangedEvent();
        return true;
    }

    private void InvokeFormationChangedEvent(){
        EventManager.Instance.formationChangedEvent.Invoke(this);
    }

    public void RemoveFormationElement(IFormationElement formationElement)
    {
        if (formationElements.Contains(formationElement))
        {
            formationElements.Remove(formationElement);
            if (formationElements.Count == 0){
                leader = null;
                EventManager.Instance.formationDeletedEvent.Invoke(this);
                Destroy(gameObject);
                return;
            }
            if (formationElement == leader){
                IFormationElement newLeader = formationElements.ElementAt(0);
                ResetPosition(newLeader.GetRelativePosition());
                SetLeader(newLeader);
            }
            else{
                ResetPosition(formationElement.GetRelativePosition());
            }
            formationElement.RemoveEntityFromFormation();
            InvokeFormationChangedEvent();
        }
    }

    private void OnEntityDeath(ICombatElement combatElement){
        IFormationElement formationElement = combatElement.GetFormationElement();
        if (formationElement == null){
            return;
        }
        RemoveFormationElement(formationElement);
    }

    public virtual void CalculateCapacity(){
        throw new System.NotImplementedException();
    }

    public virtual void CalculateRelativePositions(){
        throw new System.NotImplementedException();
    }
    
    private void UpdateUnitDestinations()
    {
        foreach (KeyValuePair<Vector3, IFormationElement> entry in relativePositions)
        {
            if (leader == null){
                return;
            }
            IFormationElement element = entry.Value;
            if (element == leader || element == null){
                continue;
            }
            Vector3 relativePosition = entry.Key;
            if (element.IsMoveable()){
                IMoveable moveable = element.GetMoveable();
                Debug.Assert(moveable != null, nameof(moveable) + " != null");
                moveable.SetMoveToPosition(leader.GetGameObject().transform.position + relativePosition);
            }
            
        }
    }

    private void ResetPosition(Vector3 pos){
        relativePositions[pos] = null;
    }

    public float GetOverallHp(){
        float maxHp = 0f;
        float currentHp = 0;
        foreach (IFormationElement element in formationElements){
            maxHp += element.GetMaxHP();
            currentHp += element.GetCurrentHP();
        }

        return currentHp / maxHp;
    }

    public Formation GetFormation(){
        return this;
    }

    #endregion

}
