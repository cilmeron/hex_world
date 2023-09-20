using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableMas
{
    private Material m_unselected;
    private Material m_selected;
    private Material m_leader;

    public SelectableMas(Material m_unselected, Material m_selected, Material m_leader){
        this.m_unselected = m_unselected;
        this.m_selected = m_selected;
        this.m_leader = m_leader;
    }

    public Material MUnselected => m_unselected;

    public Material MSelected => m_selected;

    public Material MLeader => m_leader;
}
