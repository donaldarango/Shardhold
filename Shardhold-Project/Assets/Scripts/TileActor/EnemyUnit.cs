using UnityEngine;

public class EnemyUnit : TileActor
{
    public int turnSpawned = -1; // default for not spawned
    //public int movementRange = 1; //Movement is in the enemystats SO
    public int terrainType = 0;

    public BasicEnemyStats enemyStats;
    [SerializeField] private int moveSpeed;

    //private void OnEnable()
    //{
    //    TileActorManager.NextRound += OnNextTurn;
    //}
    //private void OnDisable()
    //{
    //    TileActorManager.NextRound -= OnNextTurn;
    //}

    void Start()
    {

        if (tileActorStats == null)
        {
            Debug.LogError("Enemy missing base TileActorStats!");
            return;
        }

        enemyStats = tileActorStats as BasicEnemyStats; // Convert to EnemyStats to access move speed.
        SetEnemyData(enemyStats);
        SetActorData(enemyStats);
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

        //Debug.Log("Stats for " + enemyData.unitName + ":");

        //Debug.Log("Tile Actor Type: " + enemyData.actorType.ToString());

        //Debug.Log("Attack Range: " + enemyData.attackRange);
        //Debug.Log("Damage: " + enemyData.damage);
        //Debug.Log("Max Health: " + enemyData.maxHealth);

        currentHealth = enemyData.maxHealth;
        //Debug.Log("Current Health: " + currentHealth);

        moveSpeed = enemyData.moveSpeed; // Store move speed
        //Debug.Log("Enemy move speed: " + moveSpeed);
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
        else
        {
            //Debug.Log(enemyStats.name + " has movement range of: " + GetMoveSpeed());
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
            Debug.Log("Enemy is at the base and cannot move further.");
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
                else if (actor.GetTileActorType() == TileActorType.EnemyUnit)
                {
                    Debug.Log("Enemy stops because another enemy is in front.");
                    MapTile sideTile = CheckSideAvailability(currentQuadrant, currentRing, currentLane);
                    MoveToTile(sideTile);
                    return; // Find availability on the side.

                    // TEST!! Might not work. If enemy is 2 tiles away and has attack range 2, then enemy will move to side
                }
            }
        }
        // HELPER FUNCTION END

        // Call helper function for each movespeed the enemy has, checking each tile in front before they move

        // Move forward up to moveSpeed tiles if no obstruction - WHAT TO DO IF MOVESPEED > 1, CAN THEY ATTACK AND MOVE
        for (int i = 1; i <= moveSpeed; i++)
        {
            MapTile nextTile = CheckOpenTileInFront(currentTile); 
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
    }

    public MapTile CheckOpenTileInFront(MapTile currentMapTile)
    {
        int currentRingNumber = currentMapTile.GetRingNumber();
        if (currentRingNumber == 0) { Debug.Log("Enemy is in front of Base"); return null; } // if enemy is in front of base

        int currentQuadrant = (int)currentMapTile.GetQuadrant();
        int currentLaneNumber = currentMapTile.GetLaneNumber();

        MapTile frontTile = MapManager.Instance.GetTile(currentRingNumber - 1, currentLaneNumber);
        if (frontTile.GetCurrentTileActor() == null)
        {
            return frontTile;
        }
        return null;
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