using UnityEngine;

public class TrapUnit : TileActor
{
    public int turnSpawned = -1;
    public BasicTrapStats trapStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(tileActorStats == null)
        {
            Debug.LogError("Traps missing base TileActorStats!");
            return;
        }

        trapStats = tileActorStats as BasicTrapStats;

        SetActorData(); // Just call the parent, traps don't have any unique stats currently. When they do, we make a special setter.
    }

    public override void Spawn(MapTile tile)
    {
        currentTile = tile;

        spriteHandler = GetComponent<TileActorSpriteHandler>();
        spriteHandler.SetSpriteOrientation(tile.GetQuadrant());
        spriteHandler.SpawnAnimation();
    }

    public BasicTrapStats GetTrapStats()
    {
        return trapStats;
    }

    public override void Attack(TileActor target)
    {
        Debug.Log($"{gameObject.name} triggered by {target.gameObject.name}!");
        base.Attack(target);

        SoundFXManager.instance.PlaySoundFXClip(attackClip, transform, 10f);

        TakeDamage(1);
    }

    public override void Die()
    {
        currentTile.SetCurrentTrap(null);
        base.Die();
    }
}
