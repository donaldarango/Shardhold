using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


abstract class Spell : Card
{
    public abstract int damage { get; }
    public abstract int heal { get; }
    public abstract bool friendlyFire { get; }

    private void OnEnable()
    {
        MapGenerator.PlayCard += OnPlayCard;
    }

    private void OnDisable()
    {
        MapGenerator.PlayCard -= OnPlayCard;
    }

    private void OnPlayCard(HashSet<(int, int)> tiles)
    {
        base.coordSet = tiles;
        Play();
    }


    public override void Play()
    {
        if (coordSet == null) { return; }
        if(damage > 0) { damageArea(); }
        if(heal > 0) { healArea(); }
    }

    public void damageArea()
    {
        foreach(var tile in coordSet)
        {
            MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
            TileActor actor = target.GetCurrentTileActor();

            if(actor && actor.GetTileActorType() != TileActor.TileActorType.Trap && (actor.GetTileActorType() != TileActor.TileActorType.Structure || friendlyFire))
            {
                Debug.Log("Damaging " + actor.name + " for " + damage);
                actor.TakeDamage(damage); //hurt ally units if friendly fire is enabled.
            }
        }
    }

    public void healArea()
    {
        foreach (var tile in coordSet)
        {
            MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
            TileActor actor = target.GetCurrentTileActor();

            if (actor && actor.GetTileActorType() != TileActor.TileActorType.Trap && (actor.GetTileActorType() != TileActor.TileActorType.EnemyUnit || friendlyFire))
            {
                Debug.Log("healing " + actor.name + " for " + heal);
                actor.SetCurrentHealth(Math.Min(actor.tileActorStats.maxHealth, actor.GetCurrentHealth() + heal)); //no overheal. heal enemy units if friendly fire is enabled 
            }
        }
    }
}

