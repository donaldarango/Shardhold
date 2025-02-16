using UnityEngine;

public abstract class TileActor : MonoBehaviour
{
    public enum ObjType{
        placeholder
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    public TileActorType GetTileActorType()
    {
        return tileActorStats.actorType;
    }

    public void SetCurrentTile(MapTile currentTile)
    {
        this.currentTile = currentTile;
    }

    public void TakeDamage(int damageAmount)
    {
        if (tileActorStats.actorType.ToString() == "trap")
        {
            currentHealth -= 1; // Traps go down per use
        }
        else
        {
            currentHealth -= damageAmount; // Health goes down based on damage inflicted
        }

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