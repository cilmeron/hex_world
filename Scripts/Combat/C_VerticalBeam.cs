using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class C_VerticalBeam : C_Weapon{

    private VisualEffect vfx;
    private CapsuleCollider capCol;
    
    protected override void Start(){
        vfx = GetComponent<VisualEffect>();
        capCol = GetComponent<CapsuleCollider>();
        difficulty = GameManager.Instance.GetGameDifficulty();
    }
    
    public override void Attack(C_Health target,Entity owner){
        base.Attack(target,owner);
        vfx.Play();
        capCol.enabled = true;
        StartCoroutine(WaitForSeconds(5));
        Debug.Log("attacked");
    }

    private IEnumerator WaitForSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        capCol.enabled = false;
    }
    
    
}
