using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get { return _instance; } }

    private static MapManager _instance;
    private int ringCount; // rings around the map
    private int laneCount; // lanes per quadrant
    [SerializeField] private List<MapQuadrant> quadrantData = new List<MapQuadrant>();

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

    public int GetRingCount()
    {
        return ringCount;
    }

    public void SetRingCount(int ringCount)
    {
        this.ringCount = ringCount;
    }

    public int GetLaneCount()
    {
        return laneCount;
    }

    public void SetLaneCount(int laneCount)
    {
        this.laneCount = laneCount;
    }

    public void AddEnemyToTile(int quadrant, int ringNumber, int laneNumber, int enemyIndex)
    {
        MapTile tile = quadrantData[quadrant].getTile(ringNumber, laneNumber);
        TileActor ta = TileActorManager.Instance.GetTileActor(enemyIndex);
        GameObject prefab = TileActorManager.Instance.GetTAPrefab(enemyIndex); // For test purposes
        prefab.transform.position = new Vector3(tile.GetTileCenter().x, 0.35f, tile.GetTileCenter().z);
        tile.SetCurrentTileActor(ta);
        Instantiate(prefab, TileActorManager.Instance.transform);
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

    public List<TileActor> GetTileActorList()
    {
        List<TileActor> tileActorTypes = new List<TileActor>();
        for (int i = 0; i < 4; i++) // for each quadrant
        {
            List<MapTile> mapTiles = quadrantData[i].GetMapTilesList();
            for (int j = 0; j < mapTiles.Count; j++)
            {
                TileActor ta = mapTiles[j].GetCurrentTileActor();
                if (ta != null)
                {
                    tileActorTypes.Add(ta);
                }
                    
            }
        }
        return tileActorTypes;
    }

}
