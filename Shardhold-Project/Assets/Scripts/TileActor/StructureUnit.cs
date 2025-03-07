using UnityEngine;

public class StructureUnit: TileActor
{
    void Start()
    {
<<<<<<< Updated upstream

    }

    // Update is called once per frame
    void Update()
    {

=======
        if (tileActorStats == null)
        {
            Debug.LogError("Structure missing base TileActorStats!");
            return;
        }

        structureStats = tileActorStats as BasicStructureStats; // Convert to StructureStats to access move speed.

        SetActorData(); // If structure gets a unique stat like Armor, then we can call its own unique stat.
        // Maybe add Armor to structures so they take like -1 damage if they have 1 armor? Add some diversity and unique traits to Structures.
    }
    public BasicStructureStats GetStructureStats()
    {
        return structureStats;
>>>>>>> Stashed changes
    }
}
