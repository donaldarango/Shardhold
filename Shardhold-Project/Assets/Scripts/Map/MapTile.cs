using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapTile
{
    // Name for serializiation in Unity Editor
    public string name;

    [SerializeField] private Quadrant quadrant;
    [SerializeField] private int ringNumber;
    [SerializeField] private int laneNumber;
    [SerializeField] private int terrainType; // PLACEHOLDER TYPE
    [SerializeField] private TileActor currentTileActor = null;
    // TODO: Add Current Trap once traps implemented
    private Vector3 tileCenter;

    public MapTile(Quadrant quadrant, int ringNumber,  int laneNumber, Vector3 tileCenter)
    {
        this.quadrant = quadrant;
        this.ringNumber = ringNumber;
        this.laneNumber = laneNumber;
        this.tileCenter = tileCenter;
        name = $"Ring: {this.ringNumber}, Lane: {this.laneNumber}";
    }

    public Quadrant GetQuadrant()
    {
        return quadrant;
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
    public TileActor GetCurrentTileActor()
    {
        return currentTileActor;
    }
    public void SetCurrentTileActor(TileActor tileActor)
    {
        currentTileActor = tileActor;
    }
}
