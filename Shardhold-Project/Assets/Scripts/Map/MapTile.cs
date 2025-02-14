using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapTile
{
    // Name for serializiation in Unity Editor
    public string name;

    private int ringNumber;
    private int laneNumber;
    private int terrainType; // PLACEHOLDER TYPE
    [SerializeField] private TileActor currentTileActor = null;
    [SerializeField] private Vector3 tileCenter;

    public MapTile(int ringNumber,  int laneNumber, Vector3 tileCenter)
    {
        this.ringNumber = ringNumber;
        this.laneNumber = laneNumber;
        this.tileCenter = tileCenter;
        name = $"Ring: {this.ringNumber}, Lane: {this.laneNumber}";
    }

    public int GetRingNumber()
    {
        return ringNumber;
    }

    public int GetLaneNumber()
    {
        return laneNumber;
    }

    public Vector3 GetTileCenter()
    {
        return tileCenter;
    }

    public void SetCurrentTileActor(TileActor tileActor)
    {
        currentTileActor = tileActor;
    }
}
