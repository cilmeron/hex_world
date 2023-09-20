using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float timeToLive = 5f; // TTL in seconds
    [SerializeField] private ICombatElement owner;
    [SerializeField] private ICombatElement target;
    [SerializeField] private Vector3 startPos;

    private float remainingTimeToLive; // Remaining TTL in seconds

    private void Start()
    {
        remainingTimeToLive = timeToLive;
        SetProjectileDirection();
        EventManager.Instance.deathEvent.AddListener(CombatElementDeath);
    }

    private void FixedUpdate()
    {
        // Move the projectile forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        // Update the remaining TTL
        remainingTimeToLive -= Time.deltaTime;

        // Destroy the projectile if the remaining TTL is under zero
        if (remainingTimeToLive <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other){
        if (owner == null || target == null || other == null){
            Destroy(gameObject); // TODO improveable Logic
        }
        GameObject colliderGameObject = other.gameObject;
        if (owner == null || colliderGameObject == null || colliderGameObject == owner.GetGameObject()){
            return;
        }
        if (colliderGameObject.layer == LayerMask.NameToLayer("Ignore Raycast")){
            return;
        }

        ICombatElement combatElement = colliderGameObject.GetComponent<ICombatElement>();
        if (combatElement != null){
            EventManager.Instance.damageEvent.Invoke(combatElement,damage);
        }
        Destroy(gameObject);
    }


    private void SetProjectileDirection(){
        transform.position = startPos;
        Vector3 directionToTarget = target.GetGameObject().transform.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = rotation;
    }

    private void CombatElementDeath(ICombatElement e){
        if (e == owner){
            owner = null;
        }
        else if(e == target){
            target = null;
        }
    }
    
    public int Damage{
        get => damage;
        set => damage = value;
    }

    public ICombatElement Owner{
        get => owner;
        set => owner = value;
    }

    public ICombatElement Target{
        get => target;
        set => target = value;
    }

    public Vector3 StartPos{
        get => startPos;
        set => startPos = value;
    }
}