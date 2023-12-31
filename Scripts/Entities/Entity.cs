using git.Scripts.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Search;
using UnityEngine.UI;

public class Entity : MonoBehaviour, Detectable
{
    protected C_Health cHealth;
    protected C_Combat cCombat;
    protected C_Selectable cSelectable;
    protected C_Formation cFormation;
    protected C_Moveable cMoveable;
    private NetworkManager networkManager;
    private GameManager gameManager;
    public Detector detector;
    public int ID;
    [SerializeField] protected Player.Nations nation;
    [SerializeField] private int goldAmount;
    [SerializeField] private Sprite sprite;
    [SerializeField] protected int viewDistance = 15;


    private bool isHovered = false;
    protected readonly float walkThreshhold = 0.2f;

    public Animator Animator;
    protected static readonly int AnimVelocity = Animator.StringToHash("Velocity");
    protected static readonly int AnimHp = Animator.StringToHash("HP");
    protected static readonly int AnimWalk = Animator.StringToHash("Walk");
    private static readonly int Death = Animator.StringToHash("Death");


    protected virtual void Awake()
    {
        cHealth = GetComponent<C_Health>();
        cCombat = GetComponent<C_Combat>();
        cSelectable = GetComponent<C_Selectable>();
        cFormation = GetComponent<C_Formation>();
        cMoveable = GetComponent<C_Moveable>();


        detector.SetOwner(this);
        detector.SetRadius(viewDistance);
        networkManager = GameObject.Find("NetworkManager")?.GetComponent<NetworkManager>();
        gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();

        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not found or NetworkManager component is missing.");
        }

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found or GameManager component is missing.");
        }
    }

    protected virtual void Start()
    {
        gameManager.AddEntityToPlayer(this);
    }


    protected virtual void Update()
    {

    }


    public void SetPlayer(Player p)
    {
        nation = p.nation;
        GameResourceManager.AddResourceAmount(GetNation(), GameResourceManager.ResourceType.Gold, goldAmount);
    }

    public Player.Nations GetNation()
    {
        return nation;
    }

    public void MovePlayer(Vector3 pos)
    {
        if (networkManager != null)
        {
            //we only need to broadcast this if we are broadcasting our own positions
            if (GetNation() == gameManager.player.nation)
                networkManager.SendMsg("M:" + networkManager.playername + ":" + pos.x + "," + pos.y + "," + pos.z + ":" + ID);
        }
    }

    public Sprite GetSprite()
    {
        return sprite;
    }


    public C_Health CHealth
    {
        get => cHealth;
    }

    public C_Combat CCombat
    {
        get => cCombat;
    }

    public C_Selectable CSelectable
    {
        get => cSelectable;
    }

    public C_Formation CFormation
    {
        get => cFormation;
    }

    public C_Moveable CMoveable
    {
        get => cMoveable;
    }



    public Renderer GetRenderer()
    {
        return gameObject.GetComponent<Renderer>();
    }

    public void SetMaterialToRenderer(Material material)
    {
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
            foreach (C_Selectable selectable in selectionManager.selectedDictionary.selectedTable.Values)
            {
                if (selectable.Entity.GetNation() != GameManager.Instance.player.nation) continue;
                if (selectable.Entity.CCombat != null)
                {
                    if (CHealth != null)
                    {
                        if (selectable.Entity.GetNation() != CHealth.Entity.GetNation())
                        {
                            EventManager.Instance.setTarget.Invoke(selectable.Entity.CCombat, CHealth);
                        }
                        else if (selectable.Entity is Unit unit && selectable.Entity.GetNation() == CHealth.Entity.GetNation() && cHealth.Entity is Building)
                        {
                            unit.cMoveable.SetMoveToPosition(cHealth.transform.position, true);
                            unit.Crew = true;
                            EventManager.Instance.supportBuilding.Invoke(selectable.Entity.CCombat, CHealth);
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(2)) // Middle mouse button released
        {

        }
    }

    public List<Entity> GetEntitiesInVision()
    {
        return detector.detectedObjects
            .OfType<Entity>()
            .Where(entity => entity.CHealth != null)
            .ToList();
    }

    public void RemoveEntityInVision(Entity e)
    {
        detector.detectedObjects.Remove(e);
    }

    public void DestroyEntity()
    {
        if (Animator != null)
        {
            Animator.SetTrigger(Death);
        }
        if (networkManager != null)
        {
            networkManager.SendMsg("K:" + networkManager.playername + ":0.1,0,0:" + ID);
        }
        var components = GetComponents(typeof(Component));
        foreach (var comp in components)
        {
            if (!(comp is Transform))
            {
                Destroy(comp);
            }
        }
    }



}