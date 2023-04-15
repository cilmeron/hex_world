using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selected_dictionary : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedTable = new Dictionary<int, GameObject>();

    public void addSelected(GameObject go)
    {
        int id = go.GetInstanceID();

        if (!(selectedTable.ContainsKey(id)))
        {
            selectedTable.Add(id, go);
            go.AddComponent<selection_component>();
            Debug.Log("Added " + id + " to selected dict");
        }
    }

    public void removeSelected(GameObject go)
    {
        int id = go.GetInstanceID();

        if (selectedTable.ContainsKey(id))
        {
            selectedTable.Remove(id);
            Destroy(go.GetComponent<selection_component>());
            Debug.Log("Removed " + id + " from selected dict");
        }
    }

    public void deselect(int id)
    {
        Destroy(selectedTable[id].GetComponent<selection_component>());
        selectedTable.Remove(id);
    }

    public void deselectAll()
    {
        foreach(KeyValuePair<int,GameObject> pair in selectedTable)
        {
            if(pair.Value != null)
            {
                Destroy(selectedTable[pair.Key].GetComponent<selection_component>());
            }
        }
        selectedTable.Clear();
    }
    public void selectAll()
    {
        foreach(KeyValuePair<int,GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                selectedTable.Add(pair.Key, pair.Value);
                pair.Value.AddComponent<selection_component>();
            }
        }
    }
}