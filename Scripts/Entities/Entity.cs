using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Search;
using UnityEngine.UI;

public class Entity : MonoBehaviour, ICombatElement{

    #region Fields
        [SerializeField] protected int maxHP = 100;
        [SerializeField] protected int currentHP;
        protected Vision vision;
        private HashSet<ICombatElement> combatElementsInVision  = new HashSet<ICombatElement>();
        private ICombatElement target;
        [SerializeField] private float attackTime;
        [SerializeField] private int attackDmg;
        [SerializeField] private GameObject projectile;
        private bool isAttacking = false;
        private HpSlider hpSlider;
        [SerializeField] private Transform bulletStart;
        [SerializeField] protected Player player;
        [SerializeField] private int goldAmount;
        [SerializeField] private Sprite sprite;
        
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Transform startParent;
        protected SelectableMas SelectableMas;

        private SelectionManager selectionManager;
    #endregion
    
    
    #region Selectable Implementation

        protected virtual void Awake(){
            vision = transform.GetChild(0).GetComponent<Vision>();
            hpSlider = transform.GetChild(1).GetComponent<HpSlider>();
        }
    
        protected virtual void Start(){
            EventManager.Instance.deathEvent.AddListener(CombatElementDied);
            EventManager.Instance.damageEvent.AddListener(RemoveHp);
            EventManager.Instance.playerSuccessfullyInitialized.AddListener(InitEntityAfterPlayerInit);
            //It may occur, that some players are created before the player's Start method is called.
            //Therefore, the event needs to be invoked as well as the InitEntityAfterPlayerInit function
            //needs to be called.
            if (player.PlayerMas == null){
                return;
            }
            InitEntityAfterPlayerInit(player);
            selectionManager = SelectionManager.Instance;
        }

        private void InitEntityAfterPlayerInit(Player p){
            if (player != p){
                return;
            }
            SetPlayer(player);
            player.InitializeOwnedGameObject(this);
        }
        
        protected virtual void Update(){
            if (currentHP <= 0){
                OnDeath();
            }
    
            if (target != null && !isAttacking){
                StartCoroutine(Attack());
            }
        }
    
        public virtual void SetMaterialsAndShaders(){
            
        }

        

        public bool IsSelected(){
            if (selectionManager == null){
                return false;
            }
            return selectionManager.selectedDictionary.selectedTable.Values.Contains(this);
        }
        
    #endregion
    
    
    #region IOwnership Implementation
        public void SetPlayer(Player p){
            player = p;
            GameResourceManager.AddResourceAmount(player,GameResourceManager.ResourceType.Gold,goldAmount);
        }
    
        public Player GetPlayer(){
            return player;
        }

    #endregion
    

    #region ICombatElement Implementation
        public void RemoveHp(ICombatElement e,int hpToRemove){
            if (gameObject == e.GetGameObject()){
                currentHP -= hpToRemove;
            }
        }
        
        public void AddHp(int hpToAdd){
            currentHP += hpToAdd;
        }
        public void SetHpSliderActive(bool active){
            hpSlider.IsActive = active;
        }

        public virtual void OnDeath(){
            EventManager.Instance.deathEvent.Invoke(this);
            Destroy(gameObject);
        }

        public IEnumerator Attack(){
            float timeTillAttack = attackTime;
            isAttacking = true;
            Transform t = transform;
            GameObject projectileObject = Instantiate(projectile, t.position, t.rotation);
            Projectile bullet = projectileObject.GetComponent<Projectile>();
            bullet.StartPos = bulletStart.position;
            bullet.Owner = this;
            bullet.Target = target;
            bullet.Damage = attackDmg;
    
            while (timeTillAttack > 0){
                yield return null; // wait for the next frame
                timeTillAttack -= Time.deltaTime;
            }
            isAttacking = false;
        }
        
        public void SetTarget(){
            if (combatElementsInVision.Count == 0){
                target = null;
                return;
            }
            target = combatElementsInVision.First();
        }

        public void CombatElementDied(ICombatElement combatElement){
            if (combatElementsInVision.Contains(combatElement)){
                RemoveCombatElementFromVision(combatElement);
                if (target == combatElement){
                    target = null;
                }
            }
            SetTarget();
        }


        public ICombatElement GetTarget(){
            return target;
        }

        public void AddCombatElementToVision(ICombatElement combatElement){
            combatElementsInVision.Add(combatElement);
        }

        public void RemoveCombatElementFromVision(ICombatElement combatElement){
            combatElementsInVision.Remove(combatElement);
        }

        public float GetCurrentHP(){
            return currentHP;
        }

        public float GetMaxHP(){
            return maxHP;
        }

        #endregion
    

    #region ISelectable Implemenatation
        public float GetHpPercentage(){
            return  (float)currentHP/maxHP;
        }
    
        public virtual string GetStats(){
            return currentHP + " / " + maxHP;
        }
    
        public Sprite GetSprite(){
            return sprite;
        }

        public GameObject GetGameObject(){
            return gameObject;
        }

        public Vector3 GetPosition(){
            return transform.position;
        }
    
        public SelectableMas GetSelectableMas(){
            return SelectableMas;
        }
    #endregion
    

    #region Properties Implementation

        public HashSet<ICombatElement> CombatElementsInVision{
            get => combatElementsInVision;
            set => combatElementsInVision = value;
        }
    
        public ICombatElement Target{
            get => target;
            set => target = value;
        }
    
    

    #endregion

    #region ISelectable Implementation

    public virtual IFormationElement GetFormationElement(){
        return null;
    }

    #endregion
    
}