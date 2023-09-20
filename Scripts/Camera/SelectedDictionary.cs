using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selected_dictionary : MonoBehaviour //Evtl auf Static classe reduzieren - wie GameResourceManager
{
    public Dictionary<int, ISelectable> selectedTable = new Dictionary<int, ISelectable>();

    public void addSelected(ISelectable selectable)
    {
        int id = selectable.GetGameObject().GetInstanceID();

        if (!(selectedTable.ContainsKey(id)))
        {
            selectedTable.Add(id, selectable);
            selectable.GetGameObject().AddComponent<SelectionComponent>();
        }
    }
    public void select(ISelectable selectable)
    {
        int id = selectable.GetGameObject().GetInstanceID();
        if (selectedTable.ContainsKey(id))
        {
            removeAllBut(id);
        }
        else
        {
            deselectAll();
            addSelected(selectable);
        }
    }
    public void removeSelected(ISelectable selectable)
    {
        int id = selectable.GetGameObject().GetInstanceID();

        if (selectedTable.ContainsKey(id))
        {
            selectedTable.Remove(id);
            Destroy(selectable.GetGameObject().GetComponent<SelectionComponent>());
        }
    }
    private void removeAllBut(int id)
    {
        foreach(KeyValuePair<int,ISelectable> pair in selectedTable)
        {
            if(pair.Value != null && pair.Key != id)
            {
                Destroy(selectedTable[pair.Key].GetGameObject().GetComponent<SelectionComponent>());
                selectedTable.Remove(pair.Key);
            }
        }
    }
    public void deselectAll()
    {
        foreach(KeyValuePair<int,ISelectable> pair in selectedTable)
        {
            if(pair.Value != null)
            {
                Destroy(selectedTable[pair.Key].GetGameObject().GetComponent<SelectionComponent>());
            }
        }
        selectedTable.Clear();
    }
}