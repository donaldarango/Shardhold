using System.Collections.Generic;
using UnityEngine;
using static Card;
using static MapGenerator;
public class AllyUnit : MonoBehaviour
{
    public AllyUnitStats stats;
    int currentHealth;

    public CardType cardType => CardType.Unit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
                //return to hand
                Debug.Log("ally unit hp : " + currentHealth);
                if(currentHealth <= 0)
                {
                    Debug.Log("ally unit died, resetting");
                    //Setup();
                    //discard self
                }
            }
        }

    }
}
