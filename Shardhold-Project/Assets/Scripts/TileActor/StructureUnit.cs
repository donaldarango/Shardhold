using UnityEngine;

public class StructureUnit: TileActor
{
    public int turnSpawned = -1;

    public BasicStructureStats structureStats;
    void Start()
    {
        if (tileActorStats == null)
        {
            Debug.LogError("Enemy missing base TileActorStats!");
            return;
        }

        structureStats = tileActorStats as BasicStructureStats; // Convert to StructureStats to access move speed.

        SetActorData(); // If structure gets a unique stat like Armor, then we can call its own unique stat.
        // Maybe add Armor to structures so they take like -1 damage if they have 1 armor? Add some diversity and unique traits to Structures.
    }

    public override void Spawn(MapTile tile)
    {
        currentTile = tile;

        spriteHandler = GetComponent<TileActorSpriteHandler>();
        spriteHandler.SetSpriteOrientation(tile.GetQuadrant());
        spriteHandler.StructureSpawnAnimation();
    }
    
    public BasicStructureStats GetStructureStats()
    {
        return structureStats;
    }

    public void AttackCheck()
    {
        // I figure this will get called every round as the only action the structure will take, so it will take its damage here.
        // I want this to be called when Enemies move into range, however it might be better to do it player turn..?
        if (isPoisoned)
        {
            TakeDamage(1);
        }

        if (damage == 0) { return; }

        TileActor target = DetectEnemyInFront(attackRange); // Check for first enemy in range.
        if(target != null)
        {
            Attack(target); // Attack it.
        }
    }
}
