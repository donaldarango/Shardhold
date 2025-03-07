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
    public BasicTrapStats GetTrapStats()
    {
        return trapStats;
    }

    public override void Attack(TileActor target)
    {
        base.Attack(target);

        TakeDamage(1);
    }
}
