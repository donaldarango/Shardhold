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
    public bool isShielded;
    public bool isPoisoned;

    [Header("Tile Actor Audio")]
    public AudioClip attackClip; // Play when tile actor attacks
    public AudioClip damagedClip; // Play when tile actor is hit
    public AudioClip deathClip; // Play when tile actor dies
    public AudioClip placementClip; // Play when enemy spawns or when structure/trap is placed

    [Header("Tile Actor Prefab")]
    public GameObject actorPrefab; // Prefab reference for the actor (enemy, structure, trap)
    public float yOffset; // Value to raise sprite object to prevent clipping through floor
}