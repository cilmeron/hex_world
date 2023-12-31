using git.Scripts.Components;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private SelectionManager selectionManager;
    private NetworkManager networkManager;
    public GameObject options;
    void Start()
    {
        selectionManager = SelectionManager.Instance;
        networkManager = GameObject.Find("NetworkManager")?.GetComponent<NetworkManager>();
    }

    void Update()
    {

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
                    foreach (C_Selectable cSelectable in selectionManager.selectedDictionary.selectedTable.Values)
                    {
                        if (cSelectable.Entity.GetType() == typeof(Unit))
                        {
                            C_Moveable cMove = cSelectable.Entity.CMoveable;
                            if (cMove == null)
                            {
                                continue;
                            }
                            if (cSelectable.Entity.GetNation() != GameManager.Instance.player.nation) return;
                            cMove.SetMoveToPosition(hit.point, false);
                        }
                    }
                }
            }
        }
        if (networkManager != null)
        {
            if (Input.GetKeyUp(KeyCode.Escape) && !networkManager.chatactive)
            {
                if (!options.activeInHierarchy)
                    options.SetActive(true);
                else
                    options.SetActive(false);
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (!options.activeInHierarchy)
                    options.SetActive(true);
                else
                    options.SetActive(false);
            }
        }
    }
}
