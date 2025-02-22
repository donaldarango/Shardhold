using UnityEngine;

public class EnemyUnit : TileActor
{
    public int turnSpawned = -1; // default for not spawned
    //public int movementRange = 1; //Movement is in the enemystats SO
    public int terrainType = 0;

    public BasicEnemyStats enemyStats;
    private int moveSpeed;

    void Start()
    {
        if (tileActorStats == null)
        {
            Debug.LogError("Enemy missing base TileActorStats!");
            return;
        }

        enemyStats = tileActorStats as BasicEnemyStats; // Convert to EnemyStats to access move speed.

        SetEnemyData(enemyStats);
    }

    private void Update()
    {
        // Check game state, if enemy turn then run Move()? Should be handled by GameManager or whatever, so should I make Move public/static?
    }

    public void SetEnemyData(BasicEnemyStats enemyData)
    {
        if(enemyData == null)
        {
            Debug.LogError("Attempted to set EnemyData with null reference.");
            return;
        }

        Debug.Log("Stats for " + enemyData.unitName + ":");

        Debug.Log("Tile Actor Type: " + enemyData.actorType.ToString());

        Debug.Log("Attack Range: " + enemyData.attackRange);
        Debug.Log("Damage: " + enemyData.damage);
        Debug.Log("Max Health: " + enemyData.maxHealth);

        currentHealth = enemyData.maxHealth;
        Debug.Log("Current Health: " + currentHealth);

        moveSpeed = enemyData.moveSpeed; // Store move speed
        Debug.Log("Enemy move speed: " + moveSpeed);
    }

    public int GetMoveSpeed()
    {
        return moveSpeed;
    }

    public BasicEnemyStats GetEnemyStats()
    {
        return enemyStats;
    }

    public void EnemyMove()
    {
        // Needs to check tile in front of enemy for 3 things.
        // 1. Is there a Structure? If so, attack.
        // 2. Is there another enemy? If so, move around them if possible.
        // 3. Is there a free space? If so, move up.

        // Debugging check to make sure stats are properly read for movement.
        if (enemyStats != null)
        {
            int moveRange = enemyStats.moveSpeed;
            Debug.Log("Enemy has movement range of: " + moveRange);
        }
        else
        {
            Debug.LogError("No EnemyStats assigned to this EnemyUnit.");
        }

        if (currentTile == null) return; // Ensure enemy is on valid tile

        // 1. Check tile in front.
        MapTile frontTile = MapManager.Instance.EnemyCheckOpenTileInFront(currentTile);
        if (frontTile != null)
        {
            // If front tile is empty, move forward.
            MoveToTile(frontTile);
            return;
        }

        // 2. Check if there is a structure
        StructureUnit structure = MapManager.Instance.EnemyCheckStructureInFront(currentTile);
        if (structure != null)
        {
            //Attack(structure);
            return;
        }

        // 3. Check if another enemy is in front

    }

    public void MoveEnemy()
    {
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats is not set!");
            return;
        }

        int moveSpeed = enemyStats.moveSpeed;
        MapTile currentTile = GetCurrentTile();
        if (currentTile == null)
        {
            Debug.LogError("Enemy has no assigned tile!");
            return;
        }

        int currentQuadrant = (int)currentTile.GetQuadrant();
        int currentRing = currentTile.GetRingNumber();
        int currentLane = currentTile.GetLaneNumber();

        if (currentRing == 0)
        {
            Debug.Log("Enemy is at the base and cannot move further.");
            return;
        }

        // Check tiles in front within the enemy's attack range
        for (int i = 1; i <= enemyStats.attackRange; i++)
        {
            int targetRing = currentRing - i;
            if (targetRing < 0) break; // Prevent index underflow

            MapTile frontTile = MapManager.Instance.GetTile(currentQuadrant, targetRing, currentLane);
            if (frontTile == null) continue;

            TileActor actor = frontTile.GetCurrentTileActor();
            if (actor != null)
            {
                if (actor.GetTileActorType() == TileActorType.Structure)
                {
                    Debug.Log("Enemy attacks structure!");
                    Attack((StructureUnit)actor);
                    return; // Stop moving if attacking
                }
                else if (actor.GetTileActorType() == TileActorType.EnemyUnit)
                {
                    Debug.Log("Enemy stops because another enemy is in front.");
                    MapTile sideTile = CheckSideAvailability(currentQuadrant, currentRing, currentLane);
                    MoveToTile(sideTile);
                    return; // Find availability on the side.
                }
            }
        }

        // Move forward up to moveSpeed tiles if no obstruction
        for (int i = 1; i <= moveSpeed; i++)
        {
            MapTile nextTile = MapManager.Instance.EnemyCheckOpenTileInFront(currentTile); // need to change this to be in class
            if (nextTile == null) break; // Stop if no open tile

            MoveToTile(nextTile);
        }
    }

    private void MoveToTile(MapTile newTile)
    {
        if(newTile == null)
        {
            return;
        }

        SetCurrentTile(newTile);
        transform.position = new Vector3(newTile.GetTileCenter().x, 0.35f, newTile.GetTileCenter().z);
        Debug.Log($"Enemy moved to {newTile.GetRingNumber()}, {newTile.GetLaneNumber()}");
    }

    private MapTile CheckSideAvailability(int currentQuadrant, int currentRingNumber, int currentLaneNumber)
    {
        int laneCount = MapManager.Instance.GetLaneCount();

        for (int i = 0; i < laneCount; i++)
        {
            MapTile tile = MapManager.Instance.GetTile(currentQuadrant, currentRingNumber - 1, currentQuadrant * laneCount + i);

            if (MapManager.Instance.DoesTileContainTileActor(tile) == null)
            {
                return tile;
            }
        }
        return null;
    }
}