using DG.Tweening;
using JetBrains.Annotations;
using NUnit.Framework.Constraints;
using UnityEngine;

public class EnemyUnit : TileActor
{
    public delegate void DamageBaseHandler(int damage);
    public static event DamageBaseHandler DamageBase;

    public int turnSpawned = -1; // default for not spawned
    public int terrainType = 0;

    public BasicEnemyStats enemyStats;

    [SerializeField] protected int moveSpeed;
    [SerializeField] protected AudioClip movementClip;

    void Start()
    {
        if (tileActorStats == null)
        {
            Debug.LogError("Enemy missing base TileActorStats!");
            return;
        }

        
        SetActorData();
    }


    public override void Spawn(MapTile tile)
    {
        currentTile = tile;

        spriteHandler = GetComponent<TileActorSpriteHandler>();
        spriteHandler.SetSpriteOrientation(tile.GetQuadrant());
        spriteHandler.SpawnAnimation();
    }

    public override void SetActorData()
    {
        if (actorDataSet)
        {
            return;
        }
        enemyStats = tileActorStats as BasicEnemyStats; // Convert to EnemyStats to access move speed.
        if (enemyStats == null)
        {
            Debug.LogError("Attempted to set EnemyData with null reference.");
            return;
        }
        base.SetActorData();
        
        moveSpeed = enemyStats.moveSpeed; // Store move speed

        movementClip = enemyStats.movementClip;

        actorDataSet = true;    //redundant, as this is also set in base.SetActorData(), but put here in case the code changes to not call the base function
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

        Debug.Log($"MoveEnemy() for enemy {actorName} at {currentTile.GetRingNumber()}, {currentTile.GetLaneNumber()}");

        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats is not set!");
            return;
        }

        if(isPoisoned)
        {
            TakeDamage(1);
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

        Debug.Log("For loop starting");

        // Check tiles in front within the enemy's attack range
        for (int i = 1; i <= enemyStats.attackRange; i++)
        {
            Debug.Log($"for loop iteration {i}");

            int targetRing = currentRing - i;
            Debug.Log($"currentRing: {currentRing}, i: {i}, targetRing: {targetRing}");
            if (targetRing < 0) break; // Prevent index underflow

            MapTile frontTile = MapManager.Instance.GetTile(targetRing, currentLane);
            if (frontTile == null) {
                Debug.Log($"No tile at {targetRing}, {currentLane}");
                continue;
            } 

            TileActor actor = frontTile.GetCurrentTileActor();
            TrapUnit trap = frontTile.GetCurrentTrapUnit();
            if (actor != null)
            {
                if (actor.GetTileActorType() == TileActorType.Structure)
                {
                    Debug.Log("Enemy attacks structure!");
                    Attack((StructureUnit)actor);

                    // CHECK IF ENEMY IS STILL IN A TRAP SINCE IT DOESN'T MOVE!
                    // if current tile has a trap, call trap's trigger and attack.
                    if(currentTile.GetCurrentTrapUnit())
                    {
                        currentTile.GetCurrentTrapUnit().Attack(this);
                    } 

                    return; // Stop moving if attacking
                }
            }
            else if (trap != null)
            {
                if (movementClip)
                {
                    SoundFXManager.instance.PlaySoundFXClip(movementClip, transform, 10f);
                }

                MoveToTile(frontTile); // Move onto trap & trigger it
                Debug.Log("Enemy triggers a trap!");
                trap.Attack(this);
                return; // Stop moving once trap triggers.
            }

            if (currentRing - attackRange < 0)
            {
                Debug.Log("Enemy is attacks base and does not move forwards.");
                AttackBase();

                if (currentTile.GetCurrentTrapUnit())
                {
                    currentTile.GetCurrentTrapUnit().Attack(this);
                }

                return;
            }
            else
            {
                Debug.Log($"currentRing: {currentRing}, attackRange: {attackRange}");
            }
        }

        // Move forward up to moveSpeed tiles if no obstruction - WHAT TO DO IF MOVESPEED > 1, CAN THEY ATTACK AND MOVE
        for (int i = 1; i <= moveSpeed; i++)
        {
            MapTile nextTile = CheckOpenTileInFront(currentTile); 
            if (nextTile == null) break; // Stop if no open tile

            MoveToTile(nextTile);

            if (movementClip)
            {
                SoundFXManager.instance.PlaySoundFXClip(movementClip, gameObject.transform, 10f);
            }

            // Once enemy moves to the next tile, see if there's a trap and if there is attack and stop its movement.
            if (nextTile.GetCurrentTrapUnit())
            {
                nextTile.GetCurrentTrapUnit().Attack(this);
                return;
            }

            // Notably no check for another structure or anything in here, fine for now but may need to rework this function and throw it all inside the loop
        }
    }

    public virtual void AttackBase()
    {
        DamageBase?.Invoke(damage);

        if(attackClip)
        {
            SoundFXManager.instance.PlaySoundFXClip(attackClip, gameObject.transform, 10f);
        }

    }

    public override string GetStats()
    {
        return base.GetStats() + ($"\nFROM EnemyUnit.CS MoveSpeed: {moveSpeed}, CurrentHP: {currentHealth}");
    }

    private void MoveToTile(MapTile newTile)
    {
        if(newTile == null)
        {
            return;
        }
        Debug.Log($"Enemy {name} moving from r:{currentTile.GetRingNumber()} l:{currentTile.GetLaneNumber()} -> r:{newTile.GetRingNumber()} l:{newTile.GetLaneNumber()}");
        SetCurrentTile(newTile);
        Vector3 target = new Vector3(newTile.GetTileCenter().x, 0.35f, newTile.GetTileCenter().z);
        float duration = 1.0f;
        transform.DOMove(target, duration);
    }

    public MapTile CheckOpenTileInFront(MapTile currentMapTile)
    {
        int currentRingNumber = currentMapTile.GetRingNumber();
        if (currentRingNumber == 0) { Debug.Log("Enemy is in front of Base"); return null; } // if enemy is in front of base

        int currentQuadrant = (int)currentMapTile.GetQuadrant();
        int currentLaneNumber = currentMapTile.GetLaneNumber();

        MapTile frontTile = MapManager.Instance.GetTile(currentRingNumber - 1, currentLaneNumber);
        TileActor frontTileActor = frontTile.GetCurrentTileActor();
        if (frontTileActor == null && frontTile.GetTerrain().terrainType != TerrainType.Mountain)   //mountains are impassable and must be navigated around
        {
            return frontTile;
        }
        else if ((frontTileActor != null && (frontTileActor.GetTileActorType() == TileActorType.EnemyUnit)) || (frontTile.GetTerrain().terrainType == TerrainType.Mountain))
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

            if (MapManager.Instance.DoesTileContainTileActor(tile) == null && tile.GetTerrain().terrainType != TerrainType.Mountain)
            {
                return tile;
            }
        }
        return null;
    }
}