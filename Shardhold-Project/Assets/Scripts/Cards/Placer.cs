using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Placer : Card
{
    TileActor actor;

    public override void Play()
    {
        foreach(var tile in coordSet)
        {
            int quad = (int)(tile.Item2 / 3);
            MapTile target = MapManager.Instance.GetTile(quad, tile.Item1, tile.Item2);
            if (!target.GetCurrentTileActor())
            {
                target.SetCurrentTileActor(actor);
            }
        }
    }
}
