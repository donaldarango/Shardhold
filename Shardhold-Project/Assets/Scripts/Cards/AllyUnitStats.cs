using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

[CreateAssetMenu(fileName = "AllyUnitStats", menuName = "Scriptable Objects/AllyUnitStats")]
public class AllyUnitStats : ScriptableObject
{
    [Header("Basic Card Information")]
    public Sprite cardImage;
    public string cardName;
    public TargetType targetType;
    public int range;
    public string description;
    public int damage;
    public int hp;
}

