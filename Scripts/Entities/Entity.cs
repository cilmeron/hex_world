using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using git.Scripts.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Search;
using UnityEngine.UI;

public class Entity : MonoBehaviour, Detectable, DetectorNotification{
        protected C_Health cHealth;
        protected C_Combat cCombat;
        protected C_Selectable cSelectable;
        protected C_Formation cFormation;
        protected C_Moveable cMoveable;

        public List<Entity> _entitiesInVision = new();
        public Detector detector;
        
        public CapsuleCollider Collider;
        [SerializeField] protected Player player;
        [SerializeField] private int goldAmount;
        [SerializeField] private Sprite sprite;
        [SerializeField] protected float viewDistance = 15;

    
        private bool isHovered = false;
        protected readonly float walkThreshhold = 0.2f;
        
        public Animator Animator;
        protected static readonly int AnimVelocity = Animator.StringToHash("Velocity");
        protected static readonly int AnimHp = Animator.StringToHash("HP");
        protected static readonly int AnimWalk = Animator.StringToHash("Walk");
        private static readonly int Death = Animator.StringToHash("Death");


        protected virtual void Awake(){
            cHealth = GetComponent<C_Health>();
            cCombat = GetComponent<C_Combat>();
            cSelectable = GetComponent<C_Selectable>();
            cFormation = GetComponent<C_Formation>();
            cMoveable = GetComponent<C_Moveable>();
            
            Collider = GetComponent<CapsuleCollider>();
            
            detector.SetOwner(this);
            detector.SetDetectorNotification(this);
        }
    
        protected virtual void Start(){
            EventManager.Instance.playerSuccessfullyInitialized.AddListener(InitEntityAfterPlayerInit);
            //It may occur, that some players are created before the player's Start method is called.
            //Therefore, the event needs to be invoked as well as the InitEntityAfterPlayerInit function
            //needs to be called.
            if (player == null){
                return;
            }
            if (player.PlayerLook == null){
                return;
            }
            InitEntityAfterPlayerInit(player);
        }

        private void InitEntityAfterPlayerInit(Player p){
            if (player != p){
                return;
            }
            SetPlayer(player);
            player.InitializeOwnedGameObject(this);
        }
        
        protected virtual void Update(){
            
        }


        public void SetPlayer(Player p){
            player = p;
            GameResourceManager.AddResourceAmount(player,GameResourceManager.ResourceType.Gold,goldAmount);
        }
    
        public Player GetPlayer(){
            return player;
        }

        
    
    public Sprite GetSprite(){
        return sprite;
    }


    public C_Health CHealth{
        get => cHealth;
    }

    public C_Combat CCombat{
        get => cCombat;
    }

    public C_Selectable CSelectable{
        get => cSelectable;
    }

    public C_Formation CFormation{
        get => cFormation;
    }
    
    public C_Moveable CMoveable{
        get => cMoveable;
    }
    
    
    
    public Renderer GetRenderer(){
        return gameObject.GetComponent<Renderer>();
    }

    public void SetMaterialToRenderer(Material material){
        gameObject.GetComponent<Renderer>().material = material;
    }
    
    
    private void OnMouseEnter()
    {
        if (!isHovered)
        {
            isHovered = true;
            EventManager.Instance.mouseEnteredEntity.Invoke(this);
        }
    }

    private void OnMouseExit()
    {
        if (isHovered)
        {
            isHovered = false;
            EventManager.Instance.mouseExitedEntity.Invoke(this);
        }
    }
    private void OnMouseDown()
    {
        //if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
        //{
        //    Debug.Log("Left mouse button pressed on GameObject");
        //    // Perform actions for left mouse button press
        //}
        //else if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        //{
        //    Debug.Log("Right mouse button pressed on GameObject");
        //    // Perform actions for right mouse button press
        //}else if (Input.GetMouseButtonDown(2)) // Right mouse button pressed
        //{
        //    Debug.Log("Middle mouse button pressed on GameObject");
        //    // Perform actions for middle mouse button press
        //}
    }
    private void OnMouseOver() // OnMouseDown Event funktioniert nur für LeftClick
    {
        if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            
        }
        else if (Input.GetMouseButtonUp(1)) // Right mouse button released
        {
            SelectionManager selectionManager = SelectionManager.Instance;
            foreach (C_Selectable selectable in selectionManager.selectedDictionary.selectedTable.Values){
                if (selectable.Entity.CCombat != null){
                    if (CHealth != null){
                        if (selectable.Entity.GetPlayer() != CHealth.Entity.GetPlayer()){
                                EventManager.Instance.setTarget.Invoke(selectable.Entity.CCombat,CHealth);
                        }
                    } 
                }   
            }
        }
        else if (Input.GetMouseButtonUp(2)) // Middle mouse button released
        {
            
        }
    }
    
    public virtual void DetectorNotification(Component component, Detector.DetectionManagement direction){
        Entity e = component.GetComponent<Entity>();
        if (e.CHealth == null) return;
        if (direction == Detector.DetectionManagement.Enter && !_entitiesInVision.Contains(e)){
            _entitiesInVision.Add(e);
        }else if (direction == Detector.DetectionManagement.Exit && _entitiesInVision.Contains(e)){
            _entitiesInVision.Remove(e);
        }    
    }

    public void DestroyEntity(){
        Animator.SetTrigger(Death);
        if (CCombat != null){
           Destroy(CCombat._attackDistanceDetector.gameObject.GetComponent<Detector>());
        }
        var components = GetComponents(typeof(Component));
        foreach (var comp in components){
            if(!(comp is Transform)){
                Destroy(comp);
            }
        }
    }
}