using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Formation : MonoBehaviour{
    [SerializeField] private Formation formation;
    [SerializeField] private  Vector3 relativeFormationPos = Vector3.zero;
    [SerializeField] private Sprite sprite;
    private Entity owner;

    void Awake(){
        owner = gameObject.GetComponent<Entity>();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public bool IsLeader(){
        return formation.GetLeader() == this;
    }
    
    public void RemoveEntityFromFormation(){
        formation = null;
        relativeFormationPos = Vector3.zero;
        //SetMaterialsAndShaders();
    }
    public void AddEntityToFormation(Formation f,Vector3 relativePos){
        formation = f;
        relativeFormationPos = relativePos;
        //SetMaterialsAndShaders();
    }
    
    public bool IsInFormation(){
        return formation != null;
    }

    public Formation Formation{
        get => formation;
        set => formation = value;
    }

    public Entity Entity{
        get => owner;
    }

    public Vector3 RelativeFormationPos{
        get => relativeFormationPos;
    }

    public Sprite Sprite{
        get => sprite;
    }
}
