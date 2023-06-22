using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    public SelectedDictionary selectedDictionary;
    public GlobalSelection globalSelection;
    
    void Awake()
    {
        // Check if there's another instance of this script in the scene
        if (Instance != null && Instance != this)
        {
            // Destroy the new instance, so only one instance exists
            Destroy(gameObject);
            return;
        }

        // Set the instance to this script
        Instance = this;
        selectedDictionary = gameObject.AddComponent<SelectedDictionary>();
    }

    // Start is called before the first frame update
    void Start(){
        
        globalSelection = gameObject.AddComponent<GlobalSelection>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}