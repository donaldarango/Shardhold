using System;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

[Serializable]
abstract public class Card : ScriptableObject
{
    public int id; //the int representation of the card

    public abstract TargetType type { get; }
    public abstract int range { get; }

    public abstract string cardName { get; }
    protected HashSet<(int, int)> coordSet;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    #region Gets and Sets
    public int GetId()
    {
        return id;
    }
    #endregion
    abstract public void Play();
}


