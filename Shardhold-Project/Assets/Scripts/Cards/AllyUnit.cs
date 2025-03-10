using System.Collections.Generic;
using UnityEngine;

abstract public class AllyUnit : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public abstract int health { get; set; }
    public abstract int damage { get; }

    public override CardType cardType => CardType.Unit;

    public override void Play(HashSet<(int, int)> tiles)
    {
        coordSet = tiles;
        foreach (var tile in coordSet)
        {
            MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
            TileActor actor = target.GetCurrentTileActor();

            if (actor.GetTileActorType() == TileActor.TileActorType.EnemyUnit) //attack enemy. recieve damage. return to hand
            {
                actor.TakeDamage(damage);
                health -= actor.tileActorStats.damage;
                //return to hand
            }
        }

    }

}

