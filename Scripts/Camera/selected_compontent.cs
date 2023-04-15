using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class selection_component : MonoBehaviour{
    private Entity entity;
    private Material material;
    // Start is called before the first frame update
    void Start(){
        //only Entities can be selected
        entity = GetComponent<Entity>();
        if (entity != null){
            material = entity.gameObject.GetComponent<Renderer>().material;
        }

        material = entity.selectedMaterial;
    }

    private void OnDestroy(){
        material = entity.material;
    }
}