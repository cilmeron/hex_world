using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class C_Selectable : MonoBehaviour
{
    
    private static SelectionManager _selectionManager;
    private Entity entity;
    private C_Health health;
    void Awake(){
        entity = gameObject.GetComponent<Entity>();
    }

    
    // Start is called before the first frame update
    void Start()
    {
        _selectionManager = SelectionManager.Instance;
        health = entity.CHealth;
        if (health == null){
            Destroy(this,5);
            throw new Exception("Gameobject is Selectable, but has no HP. Double check this behaviour");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public float GetHpPercentage(){
        return  health.GetCurrentHp() / health.GetMaxHp();
    }
    
    public virtual string GetStats(){
        return health.GetCurrentHp() + " / " + health.GetMaxHp();
    }
    
    public Sprite GetSprite(){
        return entity.GetSprite();
    }

    public GameObject GetGameObject(){
        return gameObject;
    }

    public Vector3 GetPosition(){
        return transform.position;
    }
    
    public bool IsSelected(){
        if (_selectionManager == null || entity.CSelectable == null){
            return false;
        }
        return _selectionManager.selectedDictionary.selectedTable.Values.Contains(entity.CSelectable);
    }
    
    public Entity Entity{
        get => entity;
    }
}
