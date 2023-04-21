using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selected_dictionary : MonoBehaviour //Evtl auf Static classe reduzieren - wie GameResourceManager
{
    public Dictionary<int, Entity> selectedTable = new Dictionary<int, Entity>();

    public void addSelected(Entity entity)
    {
        int id = entity.gameObject.GetInstanceID();

        if (!(selectedTable.ContainsKey(id)))
        {
            selectedTable.Add(id, entity);
            entity.gameObject.AddComponent<selection_component>();
        }
    }
    public void select(Entity entity)
    {
        int id = entity.gameObject.GetInstanceID();
        if (selectedTable.ContainsKey(id))
        {
            removeAllBut(id);
        }
        else
        {
            deselectAll();
            addSelected(entity);
        }
    }
    public void removeSelected(Entity entity)
    {
        int id = entity.gameObject.GetInstanceID();

        if (selectedTable.ContainsKey(id))
        {
            selectedTable.Remove(id);
            Destroy(entity.gameObject.GetComponent<selection_component>());
        }
    }
    private void removeAllBut(int id)
    {
        foreach(KeyValuePair<int,Entity> pair in selectedTable)
        {
            if(pair.Value != null && pair.Key != id)
            {
                Destroy(selectedTable[pair.Key].gameObject.GetComponent<selection_component>());
                selectedTable.Remove(pair.Key);
            }
        }
    }
    public void deselectAll()
    {
        foreach(KeyValuePair<int,Entity> pair in selectedTable)
        {
            if(pair.Value != null)
            {
                Destroy(selectedTable[pair.Key].gameObject.GetComponent<selection_component>());
            }
        }
        selectedTable.Clear();
    }
}