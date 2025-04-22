using System;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
abstract public class Card : ScriptableObject
{
    public delegate void PlayCardHandler(HashSet<(int, int)> tiles, Card card);
    public static event PlayCardHandler PlayCard;

    public int id; //the int representation of the card

    [Header("Basic Card Information")]
    public Sprite cardImage;
    public string cardName;
    public TargetType targetType;
    public int range;
    public string description;
    public int damage;
    public int hp;

    [Header("Audio Data")]
    [SerializeField] protected AudioClip audioClip;
    [SerializeField] protected float audioVolume;

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
    public virtual bool DiscardAfterPlay()
    {
        return true;
    }
    #endregion
    virtual public void Play(HashSet<(int, int)> tiles) { PlayCard?.Invoke(tiles, this); }
}


