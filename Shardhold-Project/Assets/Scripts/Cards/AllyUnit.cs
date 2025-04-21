using System.Collections.Generic;
using UnityEngine;
using static Card;
using static MapGenerator;
public class AllyUnit : MonoBehaviour
{
    public delegate void PlayAllyUnitHandler(HashSet<(int, int)> tiles, AllyUnit card);
    public static event PlayAllyUnitHandler PlayAllyUnit;

    public AllyUnitStats stats;
    public int currentHealth;
    public int currentAttacks; //unfinished; for later
    public AudioClip audioClip;
    [SerializeField] private CardUI cardUI;
    private bool setupComplete = false;

    public CardType cardType => CardType.Unit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if (setupComplete)
        {
            return;
        }
        cardUI = GetComponent<CardUI>();
        currentHealth = stats.hp;
        setupComplete = true;
    }

    public void Play(HashSet<(int, int)> tiles)
    {
        PlayAllyUnit?.Invoke(tiles, this);

        SoundFXManager.instance.PlaySoundFXClip(stats.audioClip, gameObject.transform, 10f);

        foreach (var tile in tiles)
        {
            MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
            TileActor actor = target.GetCurrentTileActor();

            if (actor && actor.GetTileActorType() == TileActor.TileActorType.EnemyUnit) //attack enemy. recieve damage. return to hand
            {
                PlayAllyUnitAnimation(target.GetTileCenter());
                actor.TakeDamage(stats.damage);
                currentHealth -= actor.tileActorStats.damage;
                UpdateUIHealth();
                //return to hand
                Deck.Instance.selectedCardUI.DeselectCardAnimation();
                Debug.Log("ally unit hp : " + currentHealth);
            }
        }
    }

    public void UpdateUIHealth()
    {
        cardUI.updateHealth(currentHealth);
    }

    public bool DiscardAfterPlay()
    {
        if (currentHealth <= 0) {
            return true;
        }
        return false;
    }

    private void PlayAllyUnitAnimation(Vector3 tileCoords)
    {
        Vector3 pos = tileCoords + stats.animationOffset;
        if (stats.animation != null)
        {
            GameObject obj = Instantiate(stats.animation, pos, Quaternion.identity);
            SpellAnimation spellAnimation = obj.GetComponent<SpellAnimation>();
            if (spellAnimation == null)
            {
                Debug.LogWarning("Prefab does not have SpellAnimation script attached.");
            }
        }
    }
}
