using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Look
{
    private Material m_unselected;
    private Material m_selected;
    private Material m_leader;
    private Renderer renderer;

    public C_Look(Renderer renderer,Material m_unselected, Material m_selected, Material m_leader){
        this.renderer = renderer;
        this.m_unselected = m_unselected;
        this.m_selected = m_selected;
        this.m_leader = m_leader;
    }

    public Material MUnselected => m_unselected;

    public Material MSelected => m_selected;

    public Material MLeader => m_leader;
    
    public Renderer GetRenderer(){
        return renderer;
    }
}
