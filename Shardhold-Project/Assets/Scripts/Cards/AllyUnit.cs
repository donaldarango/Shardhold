using UnityEngine;

public class AllyUnit : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int health;
    public int damage;

    public override void Play()
    {
        foreach (var tile in coordSet)
        {
            int quad = (int)(tile.Item2 / 3);
            MapTile target = MapManager.Instance.GetTile(quad, tile.Item1, tile.Item2);
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

