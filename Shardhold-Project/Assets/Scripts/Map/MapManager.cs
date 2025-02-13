using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;

    public static MapManager Instance {  get { return _instance; } }

    [SerializeField] List<MapQuadrant> quadrantData = new List<MapQuadrant>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            throw new System.Exception("An instance of this singleton already exists.");
        }
        else
        {
            _instance = this;
        }
    }

    public void AddEnemyToTile(Quadrant quadrant, int circleNumber, int sectorNumber)
    {
        //quadrantData[(int)quadrant].
    }

    public void InitializeQuadrants()
    {
        quadrantData.Clear();
        quadrantData.Add(new MapQuadrant(Quadrant.NE));
        quadrantData.Add(new MapQuadrant(Quadrant.NW));
        quadrantData.Add(new MapQuadrant(Quadrant.SW));
        quadrantData.Add(new MapQuadrant(Quadrant.SE));
    }

    // Quadrant Index is same as enum values (Ex. NE = 0, NW = 1, etc.)
    public void AddTileToQuadrant(int quadrantIndex, MapTile tile)
    {
        quadrantData[quadrantIndex].AddTile(tile);
    }

}
