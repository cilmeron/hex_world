using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMAS
{
    private Material m_unit;
    private Material m_selectedUnit;
    private Material m_leaderUnit;
    private Material m_tower;
    private Material m_selectedTower;
    private Shader s_standardShader;
    private Shader s_outlineShader;

    public PlayerMAS(Material m_unit, Material m_selectedUnit, Material m_leaderUnit, Material m_tower,
        Material m_selectedTower,Shader s_standardShader, Shader s_outlineShader){
        this.m_unit = m_unit;
        this.m_selectedUnit = m_selectedUnit;
        this.m_leaderUnit = m_leaderUnit;
        this.m_tower = m_tower;
        this.m_selectedTower = m_selectedTower;
        this.s_standardShader = s_standardShader;
        this.s_outlineShader = s_outlineShader;

    }

    public Material MUnit => m_unit;

    public Material MSelectedUnit => m_selectedUnit;

    public Material MLeaderUnit => m_leaderUnit;

    public Material MTower => m_tower;

    public Material MSelectedTower => m_selectedTower;

    public Shader SStandardShader => s_standardShader;

    public Shader SOutlineShader => s_outlineShader;
}
