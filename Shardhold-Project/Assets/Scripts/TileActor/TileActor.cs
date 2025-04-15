using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

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
    [SerializeField] protected GameObject damageIndicatorPrefab;

    protected bool actorDataSet = false;
    public abstract void Spawn(MapTile tile);

    public virtual void SetActorData()
    {
        if (actorDataSet)
        {
            return;
        }

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
        
        actorDataSet = true;
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

        if(attackClip)
        {
            SoundFXManager.instance.PlaySoundFXClip(attackClip, gameObject.transform, 10f);
        }

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
            ShowDamageIndicator(0, false);
            Debug.Log($"{gameObject.name}'s shield took the blow and shattered!");
            return;
        }

        // Damage amount is a variable, special cases like Traps will pass in a low number like 1 to reduce usage number.
        currentHealth -= damageAmount;
        ShowDamageIndicator(damageAmount, false);
        spriteHandler.SpriteDamageAnimation();

        if(damagedClip)
        {
            SoundFXManager.instance.PlaySoundFXClip(damagedClip, gameObject.transform, .3f);
        }

        Debug.Log($"{gameObject.name} took {damageAmount} damage! Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ShowDamageIndicator(int amount, bool isHealing)
    {
        if (IndicatorUIManager.Instance == null || IndicatorUIManager.Instance.damageCanvas == null)
        {
            Debug.LogError("UIManager or DamageCanvas not set!");
            return;
        }

        Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.7f, 0.7f), 0.0f, 0); // world-space offset to left & slightly up
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + offset);
        GameObject indicator = Instantiate(damageIndicatorPrefab, IndicatorUIManager.Instance.damageCanvas.transform);
        indicator.transform.position = screenPos;

        TMP_Text text = indicator.GetComponent<TMP_Text>();

        // Set text and color
        text.text = (isHealing ? "+" : "-") + Mathf.Abs(amount).ToString();
        text.color = isHealing ? Color.green : Color.red;

        float scaleFactor = Mathf.Clamp01((float)amount / 5); // between 0 and 1
        float baseScale = 1f;
        float extraScale = 0.5f;

        indicator.transform.localScale = Vector3.one * (baseScale + scaleFactor * extraScale);
    }


    public virtual void ShowStats()
    {
        Debug.Log(GetStats());
    }

    public virtual string GetStats()
    {
        return $"Name: {actorName}\nActor Type: {actorType}\nCurrentHP: {currentHealth}\nMaxHP: {maxHealth}\nAtkRange: {attackRange}\nDamage: {damage}";
    }
    
    public Sprite GetSprite() {
        return spriteHandler.GetComponent<SpriteRenderer>().sprite;
    }
    public virtual void Die()
    {
        if(deathClip)
        {
            SoundFXManager.instance.PlaySoundFXClip(deathClip, gameObject.transform, 0.5f);
        }
        // Remove Enemy from grid if necessary.
        Destroy(gameObject);
    }

    public static bool Equals(List<TileActor> taList1, List<TileActor> taList2, bool allowNull)
    {
        if(taList1 == null)
        {
            return allowNull && taList2 == null;
        }

        if (taList2 == null)
        {
            return false;
        }

        if (taList1.Count != taList2.Count)
        {
            return false;
        }

        for (int i = 0; i < taList1.Count; i++)
        {
            if (!Equals(taList1[i], taList2[i], allowNull))
            {
                return false;
            }
        }

        return true;
    }

    public static bool Equals(TileActor ta1, TileActor ta2, bool allowNull)
    {
        if (ta1 == null)
        {
            return allowNull && ta1 == null;
        }

        if(ta2 == null)
        {
            return false;
        }

        return ta1.GetStats().Equals(ta2.GetStats());
    }
}