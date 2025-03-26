using System;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using static MapGenerator;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
abstract public class Card : ScriptableObject
{
    public int id; //the int representation of the card

    [Header("Basic Card Information")]
    public Sprite cardImage;
    public string cardName;
    public TargetType targetType;
    public int range;
    public string description;
    public int damage;
    public int hp;

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
    public bool DiscardAfterPlay()
    {
        if(cardType == CardType.Spell || cardType == CardType.Placer){
            return true;
        }
        return false;
    }
    #endregion
    abstract public void Play(HashSet<(int, int)> tiles);
}


