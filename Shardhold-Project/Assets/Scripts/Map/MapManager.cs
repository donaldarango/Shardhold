using System.Collections.Generic;
using Unity.VisualScripting;
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

    public MapTile GetTile(int quadrant, int ringNumber, int laneNumber)
    {
        return quadrantData[quadrant].GetTileFromQuadrant(ringNumber, laneNumber);
    }

    public void AddEnemyToTile(int quadrant, int ringNumber, int laneNumber, int enemyIndex)
    {
        MapTile tile = GetTile(quadrant, ringNumber, laneNumber);
        TileActor ta = TileActorManager.Instance.GetTileActor(enemyIndex);
        ta.SetCurrentTile(tile);
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
        List<TileActor> tileActors = new List<TileActor>();
        for (int i = 0; i < 4; i++) // for each quadrant
        {
            List<MapTile> mapTiles = quadrantData[i].GetMapTilesList();
            for (int j = 0; j < mapTiles.Count; j++)
            {
                TileActor ta = mapTiles[j].GetCurrentTileActor();
                if (ta != null)
                {
                    tileActors.Add(ta);
                }  
            }
        }
        return tileActors;
    }

    // Returns current TileActor of specified tile, if none returns null
    public TileActor DoesTileContainTileActor(int quadrant, int ringNumber, int laneNumber) 
    {
        MapTile tile = GetTile(quadrant, ringNumber, laneNumber);
        if (tile.GetCurrentTileActor() != null)
        {
            return tile.GetCurrentTileActor();
        }
        return null;
    }

    // Returns current TileActor of specified tile, if none returns null
    public TileActor DoesTileContainTileActor(MapTile tile)
    {
        if (tile.GetCurrentTileActor() != null)
        {
            return tile.GetCurrentTileActor();
        }
        return null;
    }

    // Returns MapTile if tile in front is available, if full returns null
    public MapTile EnemyCheckOpenTileInFront(int currentQuadrant, int currentRingNumber, int currentLaneNumber)
    {
        
        if (currentRingNumber == 0) { return null; } // if enemy is in front of base

        MapTile frontTile = GetTile(currentQuadrant, currentRingNumber - 1, currentLaneNumber);
        if (frontTile.GetCurrentTileActor() == null)
        {
            return frontTile;
        }
        return null;
    }

    public MapTile EnemyCheckOpenTileInFront(MapTile currentMapTile)
    {
        int currentRingNumber = currentMapTile.GetRingNumber();
        if (currentRingNumber == 0) { Debug.Log("Base");  return null; } // if enemy is in front of base

        int currentQuadrant = (int)currentMapTile.GetQuadrant();
        int currentLaneNumber = currentMapTile.GetLaneNumber();

        MapTile frontTile = GetTile(currentQuadrant, currentRingNumber - 1, currentLaneNumber);
        if (frontTile.GetCurrentTileActor() == null)
        {
            Debug.Log("CanMoveForward");
            return frontTile;
        }
        Debug.Log("CannotMoveForward");
        return null;
    }

    // Returns first available MapTile, null if none available
    public MapTile EnemyCheckRowAvailabilityInFront(int currentQuadrant, int currentRingNumber, int currentLaneNumber)
    {
        if (currentRingNumber == 0) { return null; } // if enemy is in front of base

        for (int i = 0; i < laneCount; i++)
        {
            MapTile tile = GetTile(currentQuadrant, currentRingNumber - 1, currentQuadrant * laneCount + i);
            if (DoesTileContainTileActor(tile) == null)
            {
                return tile;
            }
        }
        return null;
    }

    public MapTile EnemyCheckRowAvailabilityInFront(MapTile currentMapTile)
    {
        int currentRingNumber = currentMapTile.GetRingNumber();
        if (currentRingNumber == 0) { return null; } // if enemy is in front of base

        int currentQuadrant = (int)currentMapTile.GetQuadrant();
        int currentLaneNumber = currentMapTile.GetLaneNumber();

        for (int i = 0; i < laneCount; i++)
        {
            MapTile tile = GetTile(currentQuadrant, currentRingNumber - 1, currentQuadrant * laneCount + i);
            if (DoesTileContainTileActor(tile) == null)
            {
                return tile;
            }
        }
        return null;
    }

    public StructureUnit EnemyCheckStructureInFront(int currentQuadrant, int currentRingNumber, int currentLaneNumber)
    {
        if (currentRingNumber == 0) { return null; } // if enemy is in front of base

        MapTile frontTile = GetTile(currentQuadrant, currentRingNumber - 1, currentLaneNumber);
        if (frontTile.GetCurrentTileActor() == null)
        {
            if ((frontTile.GetCurrentTileActor().GetTileActorType()) == TileActor.TileActorType.Structure)
            {
                return frontTile.GetCurrentTileActor() as StructureUnit;
            }
        }
        return null;
    }

    public StructureUnit EnemyCheckStructureInFront(MapTile currentMapTile)
    {
        int currentRingNumber = currentMapTile.GetRingNumber();
        if (currentRingNumber == 0) { return null; } // if enemy is in front of base

        int currentQuadrant = (int)currentMapTile.GetQuadrant();
        int currentLaneNumber = currentMapTile.GetLaneNumber();

        MapTile frontTile = GetTile(currentQuadrant, currentRingNumber - 1, currentLaneNumber);
        if (frontTile.GetCurrentTileActor() == null)
        {
            if ((frontTile.GetCurrentTileActor().GetTileActorType()) == TileActor.TileActorType.Structure)
            {
                return frontTile.GetCurrentTileActor() as StructureUnit;
            }
        }
        return null;
    }

}
