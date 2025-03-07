using System;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
abstract public class Card : ScriptableObject
{
    public int id; //the int representation of the card

    [Header("Basic Card Information")]
    public string cardName;
    public TargetType targetType;
    public int range;

    public enum CardType
    {
        Spell,
        Placer,
        Unit,
        Default
    }
    public abstract CardType cardType { get; }

    protected HashSet<(int, int)> coordSet;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    #region Gets and Sets
    public int GetId()
    {
        return id;
    }
    #endregion
    abstract public void Play(HashSet<(int, int)> tiles);
}


