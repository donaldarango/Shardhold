using System;
using UnityEngine;

[Serializable]
public abstract class Terrain
{
    public TerrainType terrainType;
    public Material terrainMaterial;
    public float damageModifier;
    public float hitChanceModifier;

    public Terrain(TerrainSO terrainData)
    {
        SetData(terrainData);
    }

    public void SetData(TerrainSO terrainData)
    {
        terrainType = terrainData.terrainType;
        terrainMaterial = terrainData.terrainMaterial;
        damageModifier = terrainData.damageModifier;
        hitChanceModifier = terrainData.hitChanceModifier;
    }

    public abstract void OnTileEnter();



}
