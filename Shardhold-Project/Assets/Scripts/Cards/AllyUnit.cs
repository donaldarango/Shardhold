using System.Collections.Generic;
using UnityEngine;
using static Card;
using static MapGenerator;
public class AllyUnit : MonoBehaviour
{
    public AllyUnitStats stats;
    public int currentHealth;
    public int currentAttacks; //unfinished; for later
    [SerializeField] private CardUI cardUI;

    public CardType cardType => CardType.Unit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardUI = GetComponent<CardUI>();
        Setup();
    }

    public void Setup()
    {
        currentHealth = stats.hp;
    }

    public void Play(HashSet<(int, int)> tiles)
    {
        foreach (var tile in tiles)
        {
            MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
            TileActor actor = target.GetCurrentTileActor();

            if (actor && actor.GetTileActorType() == TileActor.TileActorType.EnemyUnit) //attack enemy. recieve damage. return to hand
            {

                actor.TakeDamage(stats.damage);
                currentHealth -= actor.tileActorStats.damage;
                cardUI.updateHealth(currentHealth);
                //return to hand
                Debug.Log("ally unit hp : " + currentHealth);
            }
        }
    }
    public bool DiscardAfterPlay()
    {
        if (currentHealth <= 0) {
            return true;
        }
        return false;
    }
}
