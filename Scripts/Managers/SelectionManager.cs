using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    public selected_dictionary selectedDictionary;
    public global_selection globalSelection;
    
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
        selectedDictionary = gameObject.AddComponent<selected_dictionary>();
    }

    // Start is called before the first frame update
    void Start(){
        
        globalSelection = gameObject.AddComponent<global_selection>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}