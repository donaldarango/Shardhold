using UnityEngine;

// A copy of the enemy stats scriptable object, but abstract so future complex classes can inherit from it.

[CreateAssetMenu(fileName = "EnemyStatsAbstract", menuName = "Scriptable Objects/EnemyAbstract")]
public abstract class EnemyStatsAbstract : TileActorStats
{
    private void OnValidate()
    {
        actorType = TileActor.TileActorType.EnemyUnit; // Automatically sets actor type to enemy unit.
    }

    [Header("Enemy Unit Stats")]
    public int moveSpeed;
}