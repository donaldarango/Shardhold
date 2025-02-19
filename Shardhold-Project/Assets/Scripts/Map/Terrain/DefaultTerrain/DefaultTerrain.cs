using System;
using UnityEngine;

[Serializable]
public class DefaultTerrain : Terrain
{
    public DefaultTerrain(TerrainSO terrainData) : base(terrainData)
    {

    }

    public override void OnTileEnter()
    {
        throw new System.NotImplementedException();
    }
}
