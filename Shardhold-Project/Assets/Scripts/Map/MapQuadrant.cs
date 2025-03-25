using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public enum Quadrant
{
    NE,
    NW,
    SW,
    SE
}
[Serializable]
public class MapQuadrant
{
    // Name for serializiation in Unity Editor
    public string name;

    private Quadrant quadrant;
    [SerializeField] private List<MapTile> mapTiles = new List<MapTile>();

    public MapQuadrant(Quadrant quadrant)
    {
        this.quadrant = quadrant;
        this.name = quadrant.ToString();
    }

    public void AddTile(MapTile tile)
    {
        mapTiles.Add(tile);
    }

    public void RemoveTile(MapTile tile)
    {
        mapTiles.Remove(tile);
    }

    public List<MapTile> GetMapTilesList()
    {
        return mapTiles;
    }

    public MapTile GetTileFromQuadrant(int ringNumber, int laneNumber)
    {
        int laneCount = MapManager.Instance.GetLaneCount();
        int index = (ringNumber * laneCount/*3*/) + (laneNumber % laneCount);
        try
        {
            return mapTiles[index];
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogError("ERROR when getting map tile with ringNumber: " + ringNumber + ", laneNumber: " + laneNumber + ", laneCount: " + laneCount + ", resulting in index of " + index + " out of list size of " + mapTiles.Count + ". ");
            return mapTiles[0];
        }
    }

    public void RemoveAllTiles()
    {
        mapTiles.Clear();
    }
}
