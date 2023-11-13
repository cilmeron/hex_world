using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Axe : C_Weapon{

    private MeshCollider _weaponCollider;
    public float throwForce = 20f;
    public float arcHeight = 1f;
    public GameObject axePrefab;
    private bool isActiveAtStart = false;
    [SerializeField]
    public float spinSpeed = 360f; // Degrees per second

    [SerializeField]
    public Vector3 rotationAxis = Vector3.up;

    
    protected override void Awake(){
        base.Awake();
        _weaponCollider = GetComponent<MeshCollider>();
    }

    

    protected override void Start(){
        if (!isActiveAtStart){
            SetWeaponActive(false);
        }
    }
    
    public override void Attack(C_Health target,Entity owner)
    {
        base.Attack(target,owner);
        entity = owner;
        entity.Animator.SetTrigger(AnimAttack);

        // Instantiate the Axe at the current position
        GameObject axeInstance = Instantiate(axePrefab, transform.position, Quaternion.identity);
        C_Axe axe = axeInstance.GetComponent<C_Axe>();
        axe.isActiveAtStart = true;
        axe.entity = owner;
        
        // Get the Rigidbody component of the instantiated axe
        Rigidbody axeRb = axeInstance.GetComponent<Rigidbody>();
        axeRb.isKinematic = false;
        axeRb.useGravity = true;

        // Calculate the direction to the target
        Vector3 axePosition = transform.position;
        Vector3 targetPosition = target.transform.position;

        // Calculate the velocity needed to throw the axe to the target at the specified angle.
        Vector3 velocity = CalculateVelocity(targetPosition, axePosition);

        // Apply the velocity to the axe's Rigidbody
        axeRb.velocity = velocity;
        
        Destroy(axeInstance,30);
    }
    
    void Update()
    {
        if (isActiveAtStart){
             SpinAxe();
        }
        // Spin the axe
       
    }

    
    Vector3 CalculateVelocity(Vector3 target, Vector3 origin)
    {
        // Calculate distance to target
        float distance = Vector3.Distance(target, origin);

        // Calculate the maximum height of the arc
        float maxHeight = distance / 2f + arcHeight;

        // Calculate time of flight based on the throwForce
        float flightTime = Mathf.Sqrt(maxHeight * 2 / Mathf.Abs(Physics.gravity.y)) / throwForce;

        // Check if flightTime is zero to prevent division by zero
        if (flightTime == 0)
        {
            flightTime = 0.1f; // Assign a small non-zero value
        }

        // Calculate velocity
        Vector3 velocity = new Vector3((target.x - origin.x) / flightTime,
            (maxHeight - origin.y) / flightTime,
            (target.z - origin.z) / flightTime);

        return velocity;
    }

    

    public void SpinAxe()
    {
        // Rotate the axe around the specified axis at the specified speed
        transform.Rotate(rotationAxis, spinSpeed * Time.deltaTime);
    }
    
    
}