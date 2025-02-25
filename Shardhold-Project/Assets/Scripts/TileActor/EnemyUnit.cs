using UnityEngine;

public class EnemyUnit : TileActor
{
    public delegate void DamageBaseHandler(int damage);
    public static event DamageBaseHandler DamageBase;

    public int turnSpawned = -1; // default for not spawned
    public int terrainType = 0;

    public BasicEnemyStats enemyStats;

    [SerializeField]private int moveSpeed;

    void Start()
    {

        if (tileActorStats == null)
        {
            Debug.LogError("Enemy missing base TileActorStats!");
            return;
        }

        enemyStats = tileActorStats as BasicEnemyStats; // Convert to EnemyStats to access move speed.
        
        SetActorData();
    }

    private void Update()
    {
        
    }

    public override void SetActorData()
    {
        if(enemyStats == null)
        {
            Debug.LogError("Attempted to set EnemyData with null reference.");
            return;
        }
        base.SetActorData();
        
        moveSpeed = enemyStats.moveSpeed; // Store move speed
    }

    public int GetMoveSpeed()
    {
        return moveSpeed;
    }

    public BasicEnemyStats GetEnemyStats()
    {
        return enemyStats;
    }

    public override void Die()
    {
        GetCurrentTile().SetCurrentTileActor(null);
        TileActorManager.Instance.RemoveEnemyFromCurrentEnemyList(this);
        base.Die();
    }

    public void MoveEnemy()
    {
        // Needs to check tile in front of enemy for 3 things.
        // 1. Is there a Structure? If so, attack.
        // 2. Is there another enemy? If so, move around them if possible.
        // 3. Is there a free space? If so, move up.

        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats is not set!");
            return;
        }

        int moveSpeed = enemyStats.moveSpeed;
        if (currentTile == null)
        {
            Debug.LogError("Enemy has no assigned tile!");
            return;
        }

        // HELPER FUNCTION START FOR BETTER IMPLEMENTATION
        int currentQuadrant = (int)currentTile.GetQuadrant();
        int currentRing = currentTile.GetRingNumber();
        int currentLane = currentTile.GetLaneNumber();

        if (currentRing == 0)
        {
            Debug.Log("Enemy is attacks base and does not move forwards.");
            AttackBase();
            return;
        }

        // Check tiles in front within the enemy's attack range
        for (int i = 1; i <= enemyStats.attackRange; i++)
        {
            int targetRing = currentRing - i;
            if (targetRing < 0) break; // Prevent index underflow

            MapTile frontTile = MapManager.Instance.GetTile(targetRing, currentLane);
            if (frontTile == null) {
                continue;
            } 

            TileActor actor = frontTile.GetCurrentTileActor();
            if (actor != null)
            {
                if (actor.GetTileActorType() == TileActorType.Structure)
                {
                    Debug.Log("Enemy attacks structure!");
                    Attack((StructureUnit)actor);
                    return; // Stop moving if attacking
                }
            }
        }

        // Move forward up to moveSpeed tiles if no obstruction - WHAT TO DO IF MOVESPEED > 1, CAN THEY ATTACK AND MOVE
        for (int i = 1; i <= moveSpeed; i++)
        {
            MapTile nextTile = CheckOpenTileInFront(currentTile); 
            if (nextTile == null) break; // Stop if no open tile

            MoveToTile(nextTile);
        }
    }

    public void AttackBase()
    {
        DamageBase?.Invoke(damage);
    }
    public override void ShowStats() {
        base.ShowStats();
        Debug.Log(currentHealth);
        Debug.Log($"FROM EnemyUnit.CS MoveSpeed: {moveSpeed}, CurrentHP: {currentHealth}");
    }

    private void MoveToTile(MapTile newTile)
    {
        if(newTile == null)
        {
            return;
        }
        SetCurrentTile(newTile);
        transform.position = new Vector3(newTile.GetTileCenter().x, 0.35f, newTile.GetTileCenter().z);
    }

    public MapTile CheckOpenTileInFront(MapTile currentMapTile)
    {
        int currentRingNumber = currentMapTile.GetRingNumber();
        if (currentRingNumber == 0) { Debug.Log("Enemy is in front of Base"); return null; } // if enemy is in front of base

        int currentQuadrant = (int)currentMapTile.GetQuadrant();
        int currentLaneNumber = currentMapTile.GetLaneNumber();

        MapTile frontTile = MapManager.Instance.GetTile(currentRingNumber - 1, currentLaneNumber);
        TileActor frontTileActor = frontTile.GetCurrentTileActor();
        if (frontTileActor == null)
        {
            return frontTile;
        }
        else if (frontTileActor.GetTileActorType() == TileActorType.EnemyUnit)
        {
            MapTile sideTile = CheckSideAvailability(currentQuadrant, currentRingNumber, currentLaneNumber);
            return sideTile;
        }
        else
        {
            return null;
        }
    }

    private MapTile CheckSideAvailability(int currentQuadrant, int currentRingNumber, int currentLaneNumber)
    {
        int laneCount = MapManager.Instance.GetLaneCount();

        for (int i = 0; i < laneCount; i++)
        {
            MapTile tile = MapManager.Instance.GetTile(currentRingNumber - 1, currentQuadrant * laneCount + i);

            if (MapManager.Instance.DoesTileContainTileActor(tile) == null)
            {
                return tile;
            }
        }
        return null;
    }
}