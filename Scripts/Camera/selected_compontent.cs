using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class selection_component : MonoBehaviour{
    private Entity entity;
    private Renderer renderer;
    // Start is called before the first frame update
    void Start(){
        //only Entities can be selected
        entity = GetComponent<Entity>();
        if (entity != null){
            renderer = entity.gameObject.GetComponent<Renderer>();
        }
        renderer.material = entity.selectedMaterial;
        entity.SetHpSliderActive(true);
    }

    private void OnDestroy(){
        renderer.material = entity.material;
        entity.SetHpSliderActive(false);
    }
}