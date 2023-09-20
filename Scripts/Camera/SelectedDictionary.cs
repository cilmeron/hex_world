using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDictionary : MonoBehaviour //Evtl auf Static classe reduzieren - wie GameResourceManager
{
    public Dictionary<int, C_Selectable> selectedTable = new Dictionary<int, C_Selectable>();

    public void AddSelected(C_Selectable selectable)
    {
        int id = selectable.gameObject.GetInstanceID();

        if (!(selectedTable.ContainsKey(id)))
        {
            selectedTable.Add(id, selectable);
            selectable.gameObject.AddComponent<SelectionComponent>();
        }
    }
    public void Select(C_Selectable selectable)
    {
        int id = selectable.gameObject.GetInstanceID();
        if (selectedTable.ContainsKey(id))
        {
            RemoveAllBut(id);
        }
        else
        {
            DeselectAll();
            AddSelected(selectable);
        }
    }
    public void RemoveSelected(C_Selectable selectable)
    {
        int id = selectable.gameObject.GetInstanceID();

        if (selectedTable.ContainsKey(id))
        {
            selectedTable.Remove(id);
            Destroy(selectable.gameObject.GetComponent<SelectionComponent>());
        }
    }
    private void RemoveAllBut(int id)
    {
        foreach(KeyValuePair<int,C_Selectable> pair in selectedTable)
        {
            if(pair.Value != null && pair.Key != id)
            {
                Destroy(selectedTable[pair.Key].gameObject.GetComponent<SelectionComponent>());
                selectedTable.Remove(pair.Key);
            }
        }
    }
    public void DeselectAll()
    {
        foreach(KeyValuePair<int,C_Selectable> pair in selectedTable)
        {
            if(pair.Value != null)
            {
                Destroy(selectedTable[pair.Key].gameObject.GetComponent<SelectionComponent>());
            }
        }
        selectedTable.Clear();
    }
}