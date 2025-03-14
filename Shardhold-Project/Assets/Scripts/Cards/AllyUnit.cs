using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AllyUnit", menuName = "Scriptable Objects/AllyUnit")]
public class AllyUnit : Card
{
    public override CardType cardType => CardType.Unit;

    public AllyUnitStats stats;
    public int currentHealth;

    void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        currentHealth = stats.maxHealth;
        hp = currentHealth;
        damage = stats.damage;
    }

    public override void Play(HashSet<(int, int)> tiles)
    {
        coordSet = tiles;
        foreach (var tile in coordSet)
        {
            MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
            TileActor actor = target.GetCurrentTileActor();

            if (actor && actor.GetTileActorType() == TileActor.TileActorType.EnemyUnit) //attack enemy. recieve damage. return to hand
            {
                actor.TakeDamage(damage);
                currentHealth -= actor.tileActorStats.damage;
                //return to hand
                Debug.Log("ally unit hp : " + currentHealth);
                if(currentHealth <= 0)
                {
                    Debug.Log("ally unit died, resetting");
                    Setup();
                    //discard self
                }
            }
        }

    }

}

