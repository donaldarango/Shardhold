using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;
using static Card;

[CreateAssetMenu(fileName = "AllyUnitStats", menuName = "Scriptable Objects/AllyUnitStats")]
public class AllyUnitStats : ScriptableObject
{
    [Header("Basic Card Information")]
    public int id;
    public Sprite cardImage;
    public string cardName;
    public TargetType targetType;
    public int range;
    public string description;
    public int damage;
    public int hp;
    public int attacks;
    public AudioClip audioClip;

    public int GetId()
    {
        return id;
    }

    public AudioClip GetAudioClip()
    {
        return audioClip;
    }

    public CardType cardType => CardType.Unit;
}

