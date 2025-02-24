using UnityEngine;

public class StructureUnit: TileActor
{
    public int turnSpawned = -1;

    public BasicStructureStats structureStats;
    void Start()
    {
        if (tileActorStats == null)
        {
            Debug.LogError("Enemy missing base TileActorStats!");
            return;
        }

        structureStats = tileActorStats as BasicStructureStats; // Convert to StructureStats to access move speed.

        SetActorData();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public BasicStructureStats GetStructureStats()
    {
        return structureStats;
    }
}
