using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class selection_component : MonoBehaviour{
    private Entity entity;
    private Material material;
    // Start is called before the first frame update
    void Start(){
        Entity e = GetComponent<Entity>();
        if (e != null){
            entity = e;
            material = entity.material;
        }
        else{
            material = GetComponent<Renderer>().material;
        }
        material.shader = Shader.Find("Custom/S_Outline");
    }

    private void OnDestroy(){
        material.shader = Shader.Find("Standard");
    }
}