using UnityEditor;
using UnityEngine;

public abstract class TileActor : MonoBehaviour
{
    public enum ObjType
    {
        placeholder,
    }
    public enum TileActorType
    {
        EnemyUnit,
        Structure,
        Trap,
    }

    [Header("Scriptable Object Data")]
    public TileActorStats tileActorStats;
    public int currentHealth; // Keep track of this separately?
    [SerializeField] protected MapTile currentTile;

    private string actorName;
    private TileActorType actorType;
    private int maxHealth;
    private int attackRange;
    private int damage;
    private PrefabAssetType actorPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetActorData(tileActorStats);

        if (tileActorStats != null)
        {
            Debug.Log("Stats for " + tileActorStats.unitName + ":");

            Debug.Log("Tile Actor Type: " + tileActorStats.actorType.ToString());

            Debug.Log("Attack Range: " + tileActorStats.attackRange);
            Debug.Log("Damage: " + tileActorStats.damage);
            Debug.Log("Max Health: " + tileActorStats.maxHealth);

            currentHealth = tileActorStats.maxHealth;
            Debug.Log("Current Health: " + currentHealth);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActorData(TileActorStats actorData)
    {
        actorName = actorData.name;
        actorType = actorData.actorType;
        maxHealth = actorData.maxHealth;
        attackRange = actorData.attackRange;
        damage = actorData.damage;
    }

    // VIRTUAL CLASS. Structures and EnemyUnits attack similarly, Traps will need to override.
    // Any special units we make will probably override this as well.
    public virtual void Attack(TileActor target)
    {
        if (target == null) return; // Invalid target

        // Simply call take damage using the damage from the TileActor
        Debug.Log($"{gameObject.name} attacks {target.gameObject.name} for {tileActorStats.damage} damage!");
        target.TakeDamage(damage);
    }

    public TileActorType GetTileActorType()
    {
        return tileActorStats.actorType;
    }

    public void SetCurrentTile(MapTile currentTile)
    {
        this.currentTile = currentTile;
    }

    public MapTile GetCurrentTile()
    {
        return currentTile;
    }

    public void TakeDamage(int damageAmount)
    {
        // Damage amount is a variable, special cases like Traps will pass in a low number like 1 to reduce usage number.
        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} took {damageAmount} damage! Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Remove Enemy from grid if necessary.
        Destroy(gameObject);
    }
}