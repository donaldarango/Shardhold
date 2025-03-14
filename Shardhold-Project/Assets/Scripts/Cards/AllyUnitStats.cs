using UnityEngine;

[CreateAssetMenu(fileName = "AllyUnitStats", menuName = "Scriptable Objects/AllyUnitStats")]
public class AllyUnitStats : ScriptableObject
{
    [Header("Ally Unit Information")]
    public string unitName;
    public int damage;
    public int maxHealth;
}