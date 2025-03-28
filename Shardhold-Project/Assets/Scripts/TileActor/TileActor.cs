using System;
using DG.Tweening;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public abstract class TileActor : MonoBehaviour
{
    
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
    [SerializeField] protected bool isShielded = false;
    [SerializeField] protected bool isPoisoned = false;

    [Header("Scriptable Object Audio")]
    [SerializeField] protected AudioClip attackClip;
    [SerializeField] protected AudioClip damagedClip;
    [SerializeField] protected AudioClip deathClip;
    [SerializeField] protected AudioClip placementClip;

    [SerializeField] protected TileActorSpriteHandler spriteHandler;
    public abstract void Spawn(MapTile tile);

    public virtual void SetActorData()
    {
        actorName = tileActorStats.unitName;
        actorType = tileActorStats.actorType;
        maxHealth = tileActorStats.maxHealth;
        currentHealth = tileActorStats.maxHealth;
        attackRange = tileActorStats.attackRange;
        damage = tileActorStats.damage;
        isShielded = tileActorStats.isShielded;
        //isPoisoned should be false by default regardless, so.

        // Audio
        attackClip = tileActorStats.attackClip;
        damagedClip = tileActorStats.damagedClip;
        placementClip = tileActorStats.placementClip;
        deathClip = tileActorStats.deathClip;
    }

    public virtual TileActor DetectEnemyInFront(int tileRange)
    {
        int currentRing = currentTile.GetRingNumber();
        int currentLane = currentTile.GetLaneNumber();

        for(int i = 1; i <= attackRange; i++)
        {
            int targetRing = currentRing + i;
            if (targetRing >= MapManager.Instance.GetRingCount()) break;

            MapTile frontTile = MapManager.Instance.GetTile(targetRing, currentLane);
            if (frontTile == null) continue;

            TileActor actor = frontTile.GetCurrentTileActor();
            if(actor != null && actor.GetTileActorType() == TileActorType.EnemyUnit)
            {
                return actor; // First enemy in range.
            }
        }

        return null; // No enemies in range.
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
    public bool GetIsShielded()
    {
        return isShielded;
    }
    public void SetIsShielded(bool value)
    {
        isShielded = value;
    }
    public bool GetIsPoisoned()
    {
        return isPoisoned;
    }
    public void SetIsPoisoned(bool value)
    {
        isPoisoned = value;
    }

    public void TakeDamage(int damageAmount)
    {
        if(isShielded)
        {
            isShielded = false;
            Debug.Log($"{gameObject.name}'s shield took the blow and shattered!");
            return;
        }

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
    
    public Sprite GetSprite() {
        return spriteHandler.GetComponent<SpriteRenderer>().sprite;
    }
    public virtual void Die()
    {
        // Remove Enemy from grid if necessary.
        Destroy(gameObject);
    }

}