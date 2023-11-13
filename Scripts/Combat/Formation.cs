using System.Collections;
using System.Collections.Generic;
using System.Linq;
using git.Scripts.Components;
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
        protected C_Formation leader;
        protected Dictionary<Vector3,C_Formation> relativePositions = new Dictionary<Vector3, C_Formation>();
        protected HashSet<C_Formation> formationElements = new HashSet<C_Formation>();
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

        UpdateFormationElementDestinations();
    }
    public void Initialize(int spacing){
        SetSpacing(spacing);
        CalculateRelativePositions();
    }

    #endregion

    #region IFormation
 public void SetLeader(C_Formation formationElement)
    {
        leader = formationElement;
        formationElements.Add(formationElement);
        formationElement.AddEntityToFormation(this,Vector3.zero);
        InvokeFormationChangedEvent();
    }

    public C_Formation GetLeader()
    {
        return leader;
    }

    public Dictionary<Vector3,C_Formation> GetRelativePositions(){
        return relativePositions;
    }
    
    public HashSet<C_Formation> GetFormationElements(){
        return formationElements;
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

    public void SetFormationElements(List<C_Formation> formationElements)
    {
        //this.units = units;
        foreach (C_Formation formationElement in formationElements){
            AddFormationElement(formationElement);
        }
    }

    public bool AddFormationElement(C_Formation e)
    {
        if (formationElements.Count == maxCapacity)
        {
            return false;
        }

        if (formationElements.Count == 0)
        {
            SetLeader(e);
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

        return AddFormationElementAt(e, relativePosition);
    }

    public bool AddFormationElementAt(C_Formation formationElement, Vector3 relPos){
        // Add the unit to the dictionary at the relative position
        if (!relativePositions.Keys.Contains(relPos)){
            return false;
        }

        if (relativePositions[relPos] != null){
            C_Formation formationElementToRemove = relativePositions[relPos];
            RemoveFormationElement(formationElementToRemove);
        }
        
        relativePositions[relPos] = formationElement;
        formationElements.Add(formationElement);
        formationElement.AddEntityToFormation(this,relPos);
        InvokeFormationChangedEvent();
        return true;
    }

    private void InvokeFormationChangedEvent(){
        EventManager.Instance.formationChangedEvent.Invoke(this);
    }

    public void RemoveFormationElement(C_Formation formationElement)
    {
        if (formationElement == null){
            return;
        }
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
                C_Formation newLeader = formationElements.ElementAt(0);
                ResetPosition(newLeader.RelativeFormationPos);
                SetLeader(newLeader);
            }
            else{
                ResetPosition(formationElement.RelativeFormationPos);
            }
            formationElement.RemoveEntityFromFormation();
            InvokeFormationChangedEvent();
        }
    }

    private void OnEntityDeath(C_Health cHealth){
        C_Formation formationElement = cHealth.Entity.CFormation;
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
    
    private void UpdateFormationElementDestinations()
    {
        foreach (KeyValuePair<Vector3, C_Formation> entry in relativePositions)
        {
            if (leader == null){
                return;
            }
            C_Formation element = entry.Value;
            if (element == leader || element == null){
                continue;
            }
            Vector3 relativePosition = entry.Key;
            if (element.Entity.CMoveable != null){
                C_Moveable moveable = element.Entity.CMoveable;
                Debug.Assert(moveable != null, nameof(moveable) + " != null");
                moveable.SetMoveToPosition(leader.transform.position + relativePosition,true);
            }
            
        }
    }

    private void ResetPosition(Vector3 pos){
        relativePositions[pos] = null;
    }

    public float GetOverallHp(){
        float maxHp = 0f;
        float currentHp = 0;
        foreach (C_Formation element in formationElements){
            maxHp += element.Entity.CHealth.GetMaxHp();
            currentHp += element.Entity.CHealth.GetCurrentHp();
        }

        return currentHp / maxHp;
    }

    public Formation GetFormation(){
        return this;
    }

    #endregion

}
