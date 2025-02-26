using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


abstract class Placer : Card
{
    public abstract TileActor actor { get; }

    public override void Play()
    {
        foreach(var tile in coordSet)
        {
            MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
            if (!target.GetCurrentTileActor())
            {
                target.SetCurrentTileActor(actor);
            }
        }
    }
}
