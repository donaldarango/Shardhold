using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Placer", menuName = "Scriptable Objects/Placer")]
class Placer : Card
{
    [Header("Placer Attributes")]
    public TileActorStats stats;
    public bool isTrap;
    public override CardType cardType => CardType.Placer;
    public override void Play(HashSet<(int, int)> tiles)
    {
        coordSet = tiles;
        foreach(var tile in coordSet)
        {
            MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
            //if (!target.GetCurrentTileActor())
            //{
            Debug.Log("placer called, placing " + stats.unitName);

            var actor = target.GetCurrentTileActor();
            if (!actor && !isTrap)
            {
                MapManager.Instance.AddStructureToMapTile(tile.Item1, tile.Item2, stats.unitName);
            }
            else if (!actor && isTrap)
            {
                MapManager.Instance.AddTrapToMapTile(tile.Item1, tile.Item2, stats.unitName);
            }
        }
    }
}
