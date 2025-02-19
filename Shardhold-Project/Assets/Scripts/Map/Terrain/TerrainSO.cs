using UnityEngine;

public enum TerrainType
{
    Default, // Grass
    Rocky,
    Flooded,
    Mountain,
    Crater
}

[CreateAssetMenu(fileName = "TerrainData", menuName = "MapData/Terrain Data")]
public class TerrainSO : ScriptableObject
{
    public TerrainType terrainType = TerrainType.Default;
    public Material terrainMaterial;
    public float damageModifier = 1.0f;
    public float hitChanceModifier = 1.0f;
}
