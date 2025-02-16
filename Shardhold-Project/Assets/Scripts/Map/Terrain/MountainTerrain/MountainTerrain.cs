using UnityEngine;

public class MountainTerrain : Terrain
{
    public MountainTerrain(TerrainSO terrainData) : base(terrainData)
    {

    }

    public override void OnTileEnter()
    {
        throw new System.NotImplementedException();
    }
}
