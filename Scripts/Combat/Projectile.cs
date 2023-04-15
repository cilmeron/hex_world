using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float timeToLive = 5f; // TTL in seconds
    [SerializeField] private Entity owner;
    [SerializeField] private Entity target;

    private float remainingTimeToLive; // Remaining TTL in seconds

    private void Start()
    {
        remainingTimeToLive = timeToLive;
        SetProjectileDirection();
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
        GameObject colliderGameObject = other.gameObject;
        if (colliderGameObject == owner.gameObject){
            return;
        }
        if (colliderGameObject.layer == LayerMask.NameToLayer("IgnoreProjectiles")){
            return;
        }

        Entity entity = colliderGameObject.GetComponent<Entity>();
        if (entity != null){
            entity.RemoveHp(damage);
        }
        Destroy(gameObject);
    }


    private void SetProjectileDirection(){
        Vector3 directionToTarget = target.transform.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = rotation;
    }

    public int Damage{
        get => damage;
        set => damage = value;
    }

    public Entity Owner{
        get => owner;
        set => owner = value;
    }

    public Entity Target{
        get => target;
        set => target = value;
    }
}