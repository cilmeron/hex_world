using UnityEngine;
using UnityEngine.Events;

public class DeathEvent : UnityEvent<ICombatElement> { }
public class DamageEvent : UnityEvent<ICombatElement, int> { }
public class FormationChanged : UnityEvent<IFormation> { }
public class FormationDeleted : UnityEvent<IFormation> { }
public class PlayerInitialized : UnityEvent<Player> { }

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public DeathEvent deathEvent;
    public DamageEvent damageEvent;
    public FormationChanged formationChangedEvent;
    public FormationDeleted formationDeletedEvent;
    public PlayerInitialized playerSuccessfullyInitialized;

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
        
    }
}