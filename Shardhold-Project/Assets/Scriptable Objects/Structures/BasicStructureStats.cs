using UnityEngine;

[CreateAssetMenu(fileName = "BasicStructureStats", menuName = "Scriptable Objects/Structures")]
public class BasicStructureStats : TileActorStats
{
    private void OnValidate()
    {
        actorType = TileActor.TileActorType.Structure; // Automatically sets actor type to enemy unit.
    }
    
}
