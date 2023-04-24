using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class Formation : MonoBehaviour, IFormation
{
    
    [SerializeField] protected Unit leader;
    [SerializeField] protected Dictionary<Vector3,Unit> relativePositions = new Dictionary<Vector3, Unit>();
    [SerializeField] protected HashSet<Unit> units = new HashSet<Unit>();
    protected Vector3 targetPos;
    protected int spacingBetweenUnits;
    [SerializeField] protected int maxCapacity;

    protected virtual void Awake(){
        EventManager.Instance.deathEvent.AddListener(OnEntityDeath);
        GameManager.Instance.player.AddFormation(this);
    }
    

// Update is called once per frame
    protected void Update()
    {
        if (units.Count == 0){
            Destroy(gameObject);
            EventManager.Instance.formationDeletedEvent.Invoke(this);
            return;
        }

        UpdateUnitDestinations();
    }

    public void Initialize(int spacing){
        SetSpacing(spacing);
        CalculateRelativePositions();
    }

    public void SetLeader(Unit u)
    {
        leader = u;
        leader.material = u.leaderMaterial;
        leader.GetComponent<Renderer>().material = leader.material;
        units.Add(u);
        u.AddUnitToFormation(this,Vector3.zero);
        InvokeFormationChangedEvent();
    }

    public Unit GetLeader()
    {
        return leader;
    }

    public Dictionary<Vector3,Unit> GetRelativePositions(){
        return relativePositions;
    }
    
    public List<Unit> GetUnitsInFormation(){
        return units.ToList();
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

    public void SetUnitsInFormation(List<Unit> units)
    {
        //this.units = units;
        foreach (Unit unit in units){
            AddUnitToFormation(unit);
        }
    }

    public bool AddUnitToFormation(Unit u)
    {
        if (units.Count == maxCapacity)
        {
            return false;
        }

        if (units.Count == 0)
        {
            SetLeader(u);
            return true;
        }
        
        //TODO REMOVE UNIT FROM OLD FORMATION

        // Find the first available relative position
        Vector3 relativePosition = Vector3.zero;
        foreach (Vector3 pos in relativePositions.Keys)
        {
            if (!relativePositions[pos])
            {
                relativePosition = pos;
                break;
            }
        }
        // Add the unit to the dictionary at the relative position
        relativePositions[relativePosition] = u;
        units.Add(u);
        u.AddUnitToFormation(this,relativePosition);
        InvokeFormationChangedEvent();
        return true;
    }

    private void InvokeFormationChangedEvent(){
        EventManager.Instance.formationChangedEvent.Invoke(this);
    }

    public void RemoveUnitFromFormation(Unit u)
    {
        if (units.Contains(u))
        {
            units.Remove(u);
            u.RemoveUnitFromFormation();
            if (units.Count == 0){
                leader = null;
                Destroy(gameObject);
                return;
            }
            if (u == leader){
                Unit newLeader = units.ElementAt(0);
                ResetPosition(newLeader.relativeFormationPos);
                SetLeader(newLeader);
            }
            else{
                ResetPosition(u.relativeFormationPos);
            }
            InvokeFormationChangedEvent();
        }
    }

    private void OnEntityDeath(Entity e)
    {
        if (e.GetType() != typeof(Unit)){
            return;
        }
        Unit u = (Unit) e;
        RemoveUnitFromFormation(u);
    }

    public virtual void CalculateCapacity(){
        throw new System.NotImplementedException();
    }

    public virtual void CalculateRelativePositions(){
        throw new System.NotImplementedException();
    }
    
    private void UpdateUnitDestinations()
    {
        foreach (KeyValuePair<Vector3, Unit> entry in relativePositions)
        {
            if (leader == null){
                return;
            }
            Unit unit = entry.Value;
            if (unit == leader || unit == null){
                continue;
            }
            Vector3 relativePosition = entry.Key;
            unit.MovePosition = leader.transform.position + relativePosition;
        }
    }

    private void ResetPosition(Vector3 pos){
        relativePositions[pos] = null;
    }

    public float GetOverallHp(){
        float maxHP = 0f;
        float currentHP = 0;
        foreach (Entity entity in units){
            maxHP += entity.MAXHp;
            currentHP += entity.CurrentHp;
        }

        return currentHP / maxHP;
    }
    
}
