using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class InputManager : MonoBehaviour{

    private SelectionManager selectionManager;
    
    void Start(){
        selectionManager = SelectionManager.Instance;
    }
    
    void Update(){
        
        // Check if the selected dictionary has any keys
        if (selectionManager.selectedDictionary.selectedTable.Keys.Count > 0)
        {
            // Check if the space key was just pressed down
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Create a ray from the camera to the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Declare a variable to store the hit position
                Vector3 hitPosition = Vector3.zero;

                // Check if the ray intersects with a collider in the scene
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    foreach(Entity e in selectionManager.selectedDictionary.selectedTable.Values){
                        if (e.GetType() == typeof(Unit)){
                            Unit u = (Unit) e;
                            u.MovePosition = hit.point;
                        }
                        
                    }
                }
            }
        }
    }
}
