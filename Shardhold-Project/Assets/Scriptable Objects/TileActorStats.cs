using UnityEngine;

[CreateAssetMenu(fileName = "TileActorStats", menuName = "Scriptable Objects/TileActorStats")]
public abstract class TileActorStats : ScriptableObject
{
    [Header("Tile Actor Information")]
    public string unitName;
    public TileActor.TileActorType actorType;

    [Header("Tile Actor Base Stats")]
    public int maxHealth;
    public int attackRange;
    public int damage;

    [Header("Tile Actor Prefab")]
    public GameObject actorPrefab; // Prefab reference for the actor (enemy, structure, trap)
}