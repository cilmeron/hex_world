using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class MaterialManager : MonoBehaviour{

    public Dictionary<String, Material> materialStorage = new Dictionary<string, Material>();
    public List<Material> materials;
    
    private static MaterialManager instance;

    public static MaterialManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        foreach (Material material in materials){
            AddMaterial(material);
        }
    }
    
    private void AddMaterial(Material material){
        materialStorage[material.name] = material;
    }

    public Material GetMaterial(String materialName){
        return materialStorage[materialName];
    }
}
