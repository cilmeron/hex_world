using UnityEngine;
using UnityEngine.Events;

public class SetTarget : UnityEvent<C_Combat,C_Health> { }
public class DeathEvent : UnityEvent<C_Health> { }
public class DamageEvent : UnityEvent<C_Health, int> { }
public class FormationChanged : UnityEvent<IFormation> { }
public class FormationDeleted : UnityEvent<IFormation> { }
public class PlayerInitialized : UnityEvent<Player> { }
public class MouseEnteredEntity : UnityEvent<Entity> { }
public class MouseExitedEntity : UnityEvent<Entity> { }


public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public DeathEvent deathEvent;
    public DamageEvent damageEvent;
    public FormationChanged formationChangedEvent;
    public FormationDeleted formationDeletedEvent;
    public PlayerInitialized playerSuccessfullyInitialized;
    public MouseEnteredEntity mouseEnteredEntity;
    public MouseExitedEntity mouseExitedEntity;
    public SetTarget setTarget;

    void Awake()
    {
        // Ensure there is only one instance of the EventManager
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize the events
        deathEvent = new DeathEvent();
        damageEvent = new DamageEvent();
        formationChangedEvent = new FormationChanged();
        formationDeletedEvent = new FormationDeleted();
        playerSuccessfullyInitialized = new PlayerInitialized();
        mouseEnteredEntity = new MouseEnteredEntity();
        mouseExitedEntity = new MouseExitedEntity();
        setTarget = new SetTarget();
        
    }
}