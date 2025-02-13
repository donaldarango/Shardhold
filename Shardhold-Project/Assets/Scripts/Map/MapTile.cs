using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapTile
{
    // Name for serializiation in Unity Editor
    public string name;

    private int circleNumber;
    private int sectorNumber;
    private Vector2 centerCoords;

    private int terrainType; // PLACEHOLDER TYPE
                             //[SerializeField] private List<EnemyUnit> enemy_units = new List<EnemyUnit>(); // PLACEHOLDER TYPE
                             //[SerializeField] private List<StructureUnit> structures = new List<StructureUnit>(); // PLACEHOLDER TYPE
    [SerializeField] private TileActor currentTileActor = null; 

    public MapTile(int circleNumber,  int sectorNumber, Vector2 centerCoords)
    {
        this.circleNumber = circleNumber;
        this.sectorNumber = sectorNumber;
        this.centerCoords = centerCoords;
        name = $"Cir: {this.circleNumber}, Sec: {this.sectorNumber}";
    }

    public int GetCircleNumber()
    {
        return circleNumber;
    }

    public int GetSectorNumber()
    {
        return sectorNumber;
    }

    public Vector2 GetCenterCoords()
    {
        return centerCoords;
    }
}
