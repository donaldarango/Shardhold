using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Spell : Card
{
    public int damage;
    public int heal;
    public bool friendlyFire = false;

    public override void Play()
    {
        if(damage > 0){ damageArea(); }
        if(heal > 0) { healArea(); }
    }

    public void damageArea()
    {
        foreach(var tile in coordSet)
        {
            MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
            TileActor actor = target.GetCurrentTileActor();

            if(actor.GetTileActorType() != TileActor.TileActorType.Trap && (actor.GetTileActorType() != TileActor.TileActorType.Structure || friendlyFire))
            {
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

            if (actor.GetTileActorType() != TileActor.TileActorType.Trap && (actor.GetTileActorType() != TileActor.TileActorType.EnemyUnit || friendlyFire))
            {
                actor.currentHealth = Math.Min(actor.tileActorStats.maxHealth, actor.currentHealth + heal); //no overheal. heal enemy units if friendly fire is enabled 
            }
        }
    }
}

