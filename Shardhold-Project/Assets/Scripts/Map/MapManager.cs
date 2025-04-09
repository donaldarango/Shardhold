using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Serializable]
    public struct EnemySpawnTile
    {
        public string name;
        public BasicEnemyStats enemyStats;
    }

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

    public void InitializeQuadrants()
    {
        quadrantData.Clear();
        quadrantData.Add(new MapQuadrant(Quadrant.NE));
        quadrantData.Add(new MapQuadrant(Quadrant.NW));
        quadrantData.Add(new MapQuadrant(Quadrant.SW));
        quadrantData.Add(new MapQuadrant(Quadrant.SE));
    }

    public MapQuadrant GetQuadrant(int index)
    {
        return quadrantData[index];
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

    public MapTile GetTile(int ringNumber, int laneNumber)
    {
        int quadrant = laneNumber / laneCount;

        return quadrantData[quadrant].GetTileFromQuadrant(ringNumber, laneNumber);
    }

    public TrapUnit AddTrapToMapTile(int ringNumber, int laneNumber, string unitName)
    {
        MapTile tile = GetTile(ringNumber, laneNumber);
        BasicTrapStats ta = TileActorManager.Instance.GetTrapTileActorByName(unitName);
        Vector3 tilePosition = new Vector3(tile.GetTileCenter().x, 0, tile.GetTileCenter().z);
        GameObject trapUnitPrefab = Instantiate(ta.actorPrefab, tilePosition, Quaternion.identity);
        trapUnitPrefab.transform.parent = TileActorManager.Instance.transform;
        TrapUnit trapUnit = trapUnitPrefab.GetComponent<TrapUnit>();
        trapUnit.Spawn(tile);
        tile.SetCurrentTrap(trapUnit);
        trapUnit.SetActorData();
        return trapUnit;
    }

    public StructureUnit AddStructureToMapTile(int ringNumber, int laneNumber, string unitName)
    {
        MapTile tile = GetTile(ringNumber, laneNumber);
        BasicStructureStats ta = TileActorManager.Instance.GetStructureTileActorByName(unitName);
        Vector3 tilePosition = new Vector3(tile.GetTileCenter().x, 0.35f, tile.GetTileCenter().z);
        GameObject structureUnitPrefab = Instantiate(ta.actorPrefab, tilePosition, Quaternion.identity);
        structureUnitPrefab.transform.parent = TileActorManager.Instance.transform;
        StructureUnit structureUnit = structureUnitPrefab.GetComponent<StructureUnit>();
        structureUnit.Spawn(tile);
        tile.SetCurrentTileActor(structureUnit);
        structureUnit.SetActorData();
        return structureUnit;
    }

    public EnemyUnit AddEnemyToMapTile(int ringNumber, int laneNumber, string unitName)
    {
        MapTile tile = GetTile(ringNumber, laneNumber);
        BasicEnemyStats ta = TileActorManager.Instance.GetEnemyTileActorByName(unitName);
        Vector3 tilePosition = new Vector3(tile.GetTileCenter().x, 0.35f, tile.GetTileCenter().z);
        GameObject enemyUnitPrefab = Instantiate(ta.actorPrefab, tilePosition, Quaternion.identity);
        enemyUnitPrefab.transform.parent = TileActorManager.Instance.transform;
        EnemyUnit enemyUnit = enemyUnitPrefab.GetComponent<EnemyUnit>();
        enemyUnit.Spawn(tile);
        tile.SetCurrentTileActor(enemyUnit);
        TileActorManager.Instance.AddEnemyToCurrentEnemyList(enemyUnit);
        enemyUnit.SetActorData();
        return enemyUnit;
    }

    // Quadrant Index is same as enum values (Ex. NE = 0, NW = 1, etc.)
    public void AddTileToQuadrant(int quadrantIndex, MapTile tile)
    {
        quadrantData[quadrantIndex].AddTile(tile);
    }

    public void RemoveAllTiles()
    {
        for (int i = 0; i < quadrantData.Count; i++)
        {
            quadrantData[i].RemoveAllTiles();
        }

        //gameObject destruction:
        for (int i = 0; i < MapGenerator.Instance.tileGameObjects.Count; i++)
        {
            Destroy(MapGenerator.Instance.tileGameObjects[i]);
        }
        MapGenerator.Instance.tileGameObjects.Clear();
    }

    public List<TileActor> GetTileActorList(bool includeTraps = false)
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
                if (includeTraps)
                {
                    TrapUnit trap = mapTiles[j].GetCurrentTrapUnit();
                    if(trap != null)
                    {
                        tileActors.Add(trap);
                    }
                }
            }
        }
        return tileActors;
    }

    public List<TrapUnit> GetTrapUnitList()
    {
        List<TrapUnit> traps = new List<TrapUnit>();
        for (int i = 0; i < 4; i++) // for each quadrant
        {
            List<MapTile> mapTiles = quadrantData[i].GetMapTilesList();
            for (int j = 0; j < mapTiles.Count; j++)
            {
                TrapUnit trap = mapTiles[j].GetCurrentTrapUnit();
                if (trap != null)
                {
                    traps.Add(trap);
                }
            }
        }
        return traps;
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

    public TileActor DoesTileContainTileActor(int ringNumber, int laneNumber)
    {
        MapTile tile = GetTile(ringNumber, laneNumber);
        if (tile.GetCurrentTileActor() != null)
        {
            return tile.GetCurrentTileActor();
        }
        return null;
    }



}
