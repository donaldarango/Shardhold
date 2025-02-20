using UnityEngine;

public class EnemyUnit : TileActor
{
    public int turnSpawned = -1; // default for not spawned
    //public int movementRange = 1; //Movement is in the enemystats SO
    public int terrainType = 0;

    private BasicEnemyStats enemyStats;
    private int moveSpeed;

    void Start()
    {
        if (enemyStats == null)
        {
            Debug.LogError("EnemyUnit missing EnemyStats!");
            return;
        }

        enemyStats = tileActorStats as BasicEnemyStats;

        SetEnemyData(enemyStats);
    }

    private void Update()
    {
        // Check game state, if enemy turn then run Move()?
    }

    public void SetEnemyData(BasicEnemyStats enemyData)
    {
        if(enemyData == null)
        {
            Debug.LogError("Attempted to set EnemyData with null reference.");
            return;
        }

        moveSpeed = enemyData.moveSpeed;
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

    // Helper function that actually moves enemy unit to a new tile, might be useful to have separate?
    private void MoveToTile(MapTile newTile)
    {
        if (newTile == null) return; // Invalid tile.

        // Update tile references.
        currentTile.SetCurrentTileActor(null); // clear previous tile.
        newTile.SetCurrentTileActor(this); // Set new tile.
        SetCurrentTile(newTile); // Sets current tile for TileActor / Enemy

        // Update GameObject position
        transform.position = new Vector3(newTile.GetTileCenter().x, 0.35f, newTile.GetTileCenter().z);
    }
}