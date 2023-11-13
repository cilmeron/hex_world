using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_ThrowingStar : C_Weapon{

    private MeshCollider _weaponCollider;
    public float throwForce = 20f;
    public GameObject throwingStarPrefab;
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

        // Instantiate the ThrowingStar at the current position
        GameObject throwingStarInstance = Instantiate(throwingStarPrefab, transform.position, Quaternion.identity);
        C_ThrowingStar throwingStar = throwingStarInstance.GetComponent<C_ThrowingStar>();
        throwingStar.isActiveAtStart = true;
        throwingStar.entity = owner;
        
        // Get the Rigidbody component of the instantiated throwing star
        Rigidbody throwingStarRb = throwingStarInstance.GetComponent<Rigidbody>();
        throwingStarRb.isKinematic = false;
        throwingStarRb.useGravity = true;

        // Calculate the direction to the target
        Vector3 throwingStarPosition = transform.position;
        Vector3 targetPosition = target.transform.position;

        // Calculate the velocity needed to throw the throwing star to the target.
        Vector3 velocity = CalculateVelocity(targetPosition, throwingStarPosition);

        // Apply the velocity to the throwing star's Rigidbody
        throwingStarRb.velocity = velocity;
        
        Destroy(throwingStarInstance,30);
    }
    
    void Update()
    {
        if (isActiveAtStart){
             SpinThrowingStar();
        }
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin)
    {
        // Calculate direction to target
        Vector3 direction = (target - origin).normalized;

        // Calculate velocity
        Vector3 velocity = direction * throwForce;

        return velocity;
    }


    public void SpinThrowingStar()
    {
        // Rotate the throwing star around the specified axis at the specified speed
        transform.Rotate(rotationAxis, spinSpeed * Time.deltaTime);
    }
}
