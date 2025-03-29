using UnityEngine;

[CreateAssetMenu(fileName = "BasicEnemyStats", menuName = "Scriptable Objects/Enemies")]
public class BasicEnemyStats : TileActorStats
{
    private void OnValidate()
    {
        actorType = TileActor.TileActorType.EnemyUnit; // Automatically sets actor type to enemy unit.
    }

    [Header("Enemy Unit Stats")]
    public int moveSpeed;

    [Header("Enemy Move Audio")]
    public AudioClip movementClip;
}