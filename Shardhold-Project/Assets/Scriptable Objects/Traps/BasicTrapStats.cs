using UnityEngine;

[CreateAssetMenu(fileName = "BasicTrapStats", menuName = "Scriptable Objects/Traps")]
public class BasicTrapStats : TileActorStats
{
    private void OnValidate()
    {
        actorType = TileActor.TileActorType.Trap; // Automatically sets actor type to trap.
    }  
    // Not sure if there are any extra stats to add to traps, maybe specialized traps but like structures most stats/functions will be within TileActors.cs
}
