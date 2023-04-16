using UnityEngine;
using UnityEngine.Events;

public class DeathEvent : UnityEvent<Entity> { }
public class DamageEvent : UnityEvent<Entity, int> { }

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public DeathEvent deathEvent;
    public DamageEvent damageEvent;

    void Awake()
    {
        // Ensure there is only one instance of the EventManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize the events
        deathEvent = new DeathEvent();
        damageEvent = new DamageEvent();
    }
}