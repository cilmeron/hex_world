using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selected_dictionary : MonoBehaviour
{
    public Dictionary<int, Entity> selectedTable = new Dictionary<int, Entity>();

    public void addSelected(Entity entity)
    {
        int id = entity.gameObject.GetInstanceID();

        if (!(selectedTable.ContainsKey(id)))
        {
            selectedTable.Add(id, entity);
            entity.gameObject.AddComponent<selection_component>();
            Debug.Log("Added " + id + " to selected dict");
        }
    }

    public void removeSelected(Entity entity)
    {
        int id = entity.gameObject.GetInstanceID();

        if (selectedTable.ContainsKey(id))
        {
            selectedTable.Remove(id);
            Destroy(entity.gameObject.GetComponent<selection_component>());
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
        foreach(KeyValuePair<int,Entity> pair in selectedTable)
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
        foreach(KeyValuePair<int,Entity> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                selectedTable.Add(pair.Key, pair.Value);
                pair.Value.gameObject.AddComponent<selection_component>();
            }
        }
    }
}