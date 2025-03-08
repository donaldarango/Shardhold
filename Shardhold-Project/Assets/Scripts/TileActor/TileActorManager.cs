using System;
using System.Collections.Generic;
using Defective.JSON;
using UnityEngine;

[Serializable]
public class TileActorManager : MonoBehaviour
{
    [Serializable]
    public struct EnemySpawnInfo
    {
        public int laneNumber;
        public BasicEnemyStats enemyUnit;
    }

    [Serializable]
    public struct RoundSpawnInfo
    {
        public int roundNumber;
        public List<EnemySpawnInfo> roundSpawnList;
    }

    // public delegate void NextRoundHandler();
    // public static event NextRoundHandler NextRound;

    private static TileActorManager _instance;

    [SerializeField] private TextAsset levelSettingsJSON;
    [SerializeField] private List<BasicEnemyStats> enemyTileActorsStats = new List<BasicEnemyStats>();
    [SerializeField] private List<BasicStructureStats> structureTileActorStats = new List<BasicStructureStats>();
    [SerializeField] private List<EnemyUnit> currentEnemyUnits = new List<EnemyUnit>();
    [SerializeField] private List<RoundSpawnInfo> gameSpawnList = new List<RoundSpawnInfo>();

    public delegate void PlayerTurnHandler();
    public static event PlayerTurnHandler PlayerTurnStart;
    public int currentRound = 0;


    public static TileActorManager Instance { get { return _instance; } }
    void OnEnable()
    {
        TurnTimer.EnemyTurnStart += OnNextRound;
    }
    void OnDisable()
    {
        TurnTimer.EnemyTurnStart -= OnNextRound;
    }

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

    void Start()
    {
        Debug.Log("Tile Actor Manager Begin");
        InitializeSpawnData();
    }

    void Update()
    {

    }

    public void InitializeSpawnData()
    {
        if (!levelSettingsJSON)
            throw new System.Exception("No Level Settings set in Inspector");

        JSONObject jsonObject = new JSONObject(levelSettingsJSON.ToString());
        var roundSpawnDataList = jsonObject["EnemySpawnData"]["Rounds"];

        for (int i = 0; i < roundSpawnDataList.list.Count; i++)
        {
            int roundNumber = roundSpawnDataList.list[i]["RoundNumber"].intValue;
            var enemyRoundData = roundSpawnDataList.list[i]["RoundSpawnList"];

            RoundSpawnInfo roundSpawnInfo = new RoundSpawnInfo();
            roundSpawnInfo.roundNumber = roundNumber;
            roundSpawnInfo.roundSpawnList = new List<EnemySpawnInfo>();
            for (int j = 0; j < enemyRoundData.list.Count; j++)
            {
                string unitName = enemyRoundData[j]["EnemyUnit"].stringValue;
                int laneNumber = enemyRoundData[j]["LaneNumber"].intValue;
                BasicEnemyStats enemyStats = GetEnemyTileActorByName(unitName);

                if (roundNumber == 0) // Add starting round enemies to spawn tiles
                {
                    MapManager.Instance.AddEnemyToSpawnTile(laneNumber, enemyStats);
                }
                else
                {
                    EnemySpawnInfo enemySpawnInfo = new EnemySpawnInfo();
                    enemySpawnInfo.enemyUnit = enemyStats;
                    enemySpawnInfo.laneNumber = laneNumber;

                    roundSpawnInfo.roundSpawnList.Add(enemySpawnInfo);
                }
                
            }
            if (roundNumber > 0) // Do not add start round enemies
            {
                gameSpawnList.Add(roundSpawnInfo);
            }
        }
    }

    public void AddUnitsToSpawnTiles(int roundNumber)
    {
        for (int i = 0; i < gameSpawnList.Count; i++)
        {
            if (gameSpawnList[i].roundNumber == roundNumber)
            {
                for (int j = 0; j < gameSpawnList[i].roundSpawnList.Count; j++)
                {
                    EnemySpawnInfo enemySpawnInfo = gameSpawnList[i].roundSpawnList[j];
                    MapManager.Instance.AddEnemyToSpawnTile(enemySpawnInfo.laneNumber, enemySpawnInfo.enemyUnit);
                }
                gameSpawnList.RemoveAt(i);
                return;
            }
        }
    }

    public void OnNextRound()
    {
        currentRound = currentRound + 1;

        foreach (EnemyUnit enemyUnit in currentEnemyUnits)
        {
            enemyUnit.MoveEnemy();
        }

        MapManager.Instance.MoveSpawnUnitsToMap();
        AddUnitsToSpawnTiles(currentRound);
        PlayerTurnStart?.Invoke();
    }

    public BasicEnemyStats GetEnemyTileActorByName(string unitName)
    {
        for (int i = 0; i < enemyTileActorsStats.Count; i++)
        {
            if (unitName == enemyTileActorsStats[i].unitName)
                return enemyTileActorsStats[i];
        }
        throw new System.Exception($"Enemy unit with name: {unitName} not found");
    }

    public BasicStructureStats GetStructureTileActorByName(string unitName)
    {
        for (int i = 0; i < structureTileActorStats.Count; i++)
        {
            if (unitName == structureTileActorStats[i].unitName)
                return structureTileActorStats[i];
        }
        throw new System.Exception($"Structure unit with name: {unitName} not found");
    }

    public void AddEnemyToCurrentEnemyList(EnemyUnit unit)
    {
        currentEnemyUnits.Add(unit);
        currentEnemyUnits.Sort((u1,u2) => { return u1.GetCurrentTile().GetRingNumber().CompareTo(u2.GetCurrentTile().GetRingNumber()); });
    }

    public void RemoveEnemyFromCurrentEnemyList(EnemyUnit unit)
    {
        currentEnemyUnits.Remove(unit);
        currentEnemyUnits.Sort((u1, u2) => { return u1.GetCurrentTile().GetRingNumber().CompareTo(u2.GetCurrentTile().GetRingNumber()); });
    }

    public List<RoundSpawnInfo> GetSpawnList()
    {
        return gameSpawnList;
    }

    public void SetSpawnList(List<RoundSpawnInfo> list)
    {
        gameSpawnList = list;
    }

}
