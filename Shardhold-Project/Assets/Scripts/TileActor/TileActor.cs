using DG.Tweening;
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
    [SerializeField]protected int currentHealth; // Keep track of this separately?
    [SerializeField]protected MapTile currentTile = null;

    [SerializeField]protected string actorName;
    [SerializeField]protected TileActorType actorType;
    [SerializeField]protected int maxHealth;
    [SerializeField]protected int attackRange;
    [SerializeField]protected int damage;
    [SerializeReference]protected PrefabAssetType actorPrefab;

    [SerializeField] protected TileActorSpriteHandler spriteHandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Spawn(MapTile tile);

    public virtual void SetActorData()
    {
        actorName = tileActorStats.unitName;
        actorType = tileActorStats.actorType;
        maxHealth = tileActorStats.maxHealth;
        currentHealth = tileActorStats.maxHealth;
        attackRange = tileActorStats.attackRange;
        damage = tileActorStats.damage;
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

    public void SetCurrentHealth(int health)
    {
        currentHealth = health;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    public string GetActorName() {
        return actorName;
    }
    public int GetMaxHealth() {
        return maxHealth;
    }
    public int GetAttackDamage() {
        return damage;
    }
    public int GetAttackRange() {
        return attackRange;
    }

    public void TakeDamage(int damageAmount)
    {
        // Damage amount is a variable, special cases like Traps will pass in a low number like 1 to reduce usage number.
        currentHealth -= damageAmount;
        spriteHandler.SpriteDamageAnimation();

        Debug.Log($"{gameObject.name} took {damageAmount} damage! Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void ShowStats()
    {
        Debug.Log($"Name: {actorName}\nActor Type: {actorType}\nCurrentHP: {currentHealth}\nMaxHP: {maxHealth}\nAtkRange: {attackRange}\nDamage: {damage}");
    }

    public virtual void Die()
    {
        // Remove Enemy from grid if necessary.
        Destroy(gameObject);
    }

}