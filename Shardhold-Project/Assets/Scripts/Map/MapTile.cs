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
    [SerializeReference] private Terrain terrain; 
    [SerializeField] private TileActor currentTileActor = null;
    [SerializeField] private TrapUnit currentTrapUnit = null;
    private Vector3 tileCenter;

    public MapTile(Quadrant quadrant, int ringNumber, int laneNumber, Vector3 tileCenter, Terrain terrain)
    {
        this.quadrant = quadrant;
        this.ringNumber = ringNumber;
        this.laneNumber = laneNumber;
        this.tileCenter = tileCenter;
        this.terrain = terrain;
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

    public Terrain GetTerrain()
    {
        return terrain;
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

    public TrapUnit GetCurrentTrapUnit()
    {
        return currentTrapUnit;
    }

    public void SetCurrentTrap(TrapUnit trap)
    {
        currentTrapUnit = trap;
    }
}
