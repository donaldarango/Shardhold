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
<<<<<<< Updated upstream

    private void Update()
    {
        // Check game state, if enemy turn then run Move()? Should be handled by GameManager or whatever, so should I make Move public/static?
    }

    public void SetEnemyData(BasicEnemyStats enemyData)
=======
    public override void SetActorData()
>>>>>>> Stashed changes
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
        else
        {
            Debug.Log(enemyStats.name + " has movement range of: " + GetMoveSpeed());
        }

        int moveSpeed = enemyStats.moveSpeed;
        MapTile currentTile = GetCurrentTile();
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
        // HELPER FUNCTION END

        // Call helper function for each movespeed the enemy has, checking each tile in front before they move

        // Move forward up to moveSpeed tiles if no obstruction - WHAT TO DO IF MOVESPEED > 1, CAN THEY ATTACK AND MOVE
        for (int i = 1; i <= moveSpeed; i++)
        {
            MapTile nextTile = MapManager.Instance.EnemyCheckOpenTileInFront(currentTile); // CHANGE TO HELPER FUNCTION
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