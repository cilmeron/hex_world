using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Projectile : C_Weapon
{
    
    
    public float throwForce = 20f;
    
    private bool isActiveAtStart = false;
    public GameObject arrowPrefab;
    public bool followGround = true;
    public float speed = 30;
    public bool slowDown;
    public float slowDownRate = 0.01f;
    public float detectingDistance = 0.1f;
    public float destroyDelay = 5;
    public float objectsToDetachDelay = 2;
    public List<GameObject> objectsToDetach = new List<GameObject>();
    [Space]
    public float erodeInRate = 0.06f;
    public float erodeOutRate = 0.03f;
    public float erodeRefreshRate = 0.01f;
    public float erodeAwayDelay = 1.25f;
    public List<SkinnedMeshRenderer> objectsToErode = new List<SkinnedMeshRenderer>();

    private Rigidbody rb;
    private bool stopped;

    void Start()
    {
        if(followGround)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
            if(slowDown)
                StartCoroutine(SlowDown());
        }
        else
            Debug.Log("No Rigidbody");
        //Destroy(gameObject, destroyDelay);
    }

    private void FixedUpdate()
    {
        if (!stopped && followGround)
        {
            RaycastHit hit;
            Vector3 distance = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            if (Physics.Raycast(distance, transform.TransformDirection(-Vector3.up), out hit, detectingDistance))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            Debug.DrawRay(distance, transform.TransformDirection(-Vector3.up * detectingDistance), Color.red);
        }
    }

    IEnumerator SlowDown ()
    {
        float t = 1;
        while (t > 0)
        {
            rb.velocity = Vector3.Lerp(Vector3.zero, rb.velocity, t);
            t -= slowDownRate;
            yield return new WaitForSeconds(0.1f);
        }

        stopped = true;
    }

    IEnumerator DetachObjects ()
    {
        yield return new WaitForSeconds(objectsToDetachDelay);

        for (int i=0; i<objectsToDetach.Count; i++)
        {
            objectsToDetach[i].transform.parent = null;
            Destroy(objectsToDetach[i], objectsToDetachDelay);
        }
    }

    IEnumerator ErodeObjects()
    {
        for (int i = 0; i < objectsToErode.Count; i++)
        {
            float t = 1;
            while (t > 0)
            {
                t -= erodeInRate;
                for (int j = 0; j < objectsToErode[i].materials.Length; j++)
                {
                    objectsToErode[i].materials[j].SetFloat("_Erode", t);
                }
                yield return new WaitForSeconds(erodeRefreshRate);
            }
        }

        yield return new WaitForSeconds(erodeAwayDelay);

        for (int i = 0; i < objectsToErode.Count; i++)
        {
            float t = 0;
            while (t < 1)
            {
                t += erodeOutRate;
                for (int j = 0; j < objectsToErode[i].materials.Length; j++)
                {
                    objectsToErode[i].materials[j].SetFloat("_Erode", t);
                }
                yield return new WaitForSeconds(erodeRefreshRate);
            }
        }
    }
    
    
    public override void Attack(C_Health target,Entity owner)
    {
        base.Attack(target,owner);
        entity = owner;
        if (entity.Animator != null){
            entity.Animator.SetTrigger(AnimAttack);
        }
       

        // Instantiate the Axe at the current position
        GameObject projectileInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        C_Projectile projectile = projectileInstance.GetComponent<C_Projectile>();
        projectile.isActiveAtStart = true;
        projectile.entity = owner;
        
        // Get the Rigidbody component of the instantiated axe
        Rigidbody axeRb = projectileInstance.GetComponent<Rigidbody>();
        axeRb.isKinematic = false;
        axeRb.useGravity = true;

        // Calculate the direction to the target
        Vector3 axePosition = transform.position;
        Vector3 targetPosition = target.transform.position;
        targetPosition.y += 1;

        // Calculate the velocity needed to throw the axe to the target at the specified angle.
        Vector3 velocity = CalculateVelocity(targetPosition, axePosition);

        // Apply the velocity to the axe's Rigidbody
        axeRb.velocity = velocity;
        
        Destroy(projectileInstance,5);
    }
    
    Vector3 CalculateVelocity(Vector3 target, Vector3 origin)
    {
        // Calculate distance to target
        float distance = Vector3.Distance(target, origin);

        // Calculate time of flight based on the throwForce
        float flightTime = distance / throwForce;

        // Check if flightTime is zero to prevent division by zero
        if (flightTime == 0)
        {
            flightTime = 0.1f; // Assign a small non-zero value
        }

        // Calculate velocity
        Vector3 velocity = (target - origin) / flightTime;

        return velocity;
    }

    
}
