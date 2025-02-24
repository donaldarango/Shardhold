using NUnit.Framework;
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
    public bool printStats = false;

    [SerializeField] private string actorName;
    [SerializeField] private TileActorType actorType;
    [SerializeField] private int maxHealth;
    [SerializeField] private int attackRange;
    [SerializeField] private int damage;
    private PrefabAssetType actorPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetActorData(tileActorStats);

        if (tileActorStats != null)
        {
            if (printStats)
            {
                Debug.Log("Stats for " + tileActorStats.unitName + ":");

                Debug.Log("Tile Actor Type: " + tileActorStats.actorType.ToString());

                Debug.Log("Attack Range: " + tileActorStats.attackRange);
                Debug.Log("Damage: " + tileActorStats.damage);
                Debug.Log("Max Health: " + tileActorStats.maxHealth);

                Debug.Log("Current Health: " + currentHealth);
            }
            currentHealth = tileActorStats.maxHealth;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void SetActorData(TileActorStats actorData)
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

        // Structures & Enemies will target other TileActors, which should be the opposite ActorType excluding traps (unless unique enemy, ie Engineer)
        if((GetTileActorType() == TileActorType.EnemyUnit && target.GetTileActorType() == TileActorType.Structure ||
            GetTileActorType() == TileActorType.Structure && target.GetTileActorType() == TileActorType.EnemyUnit))
        {
            Debug.Log($"{gameObject.name} attacks {target.gameObject.name} for {tileActorStats.damage} damage!");
            target.TakeDamage(tileActorStats.damage);
        }
    }

    public override string ToString()
    {
        return actorName;
    }

    public TileActorType GetTileActorType()
    {
        return tileActorStats.actorType;
    }

    public void SetCurrentTile(MapTile newTile)
    {
        currentTile.SetCurrentTileActor(null);
        currentTile = newTile;
        newTile.SetCurrentTileActor(this);
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

    public virtual void Die()
    {
        // Remove Enemy from grid if necessary.
        Destroy(gameObject);
    }
}