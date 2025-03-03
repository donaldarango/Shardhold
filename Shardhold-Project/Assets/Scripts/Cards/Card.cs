using System;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

[Serializable]
abstract public class Card : MonoBehaviour
{
    public int id; //the int representation of the card
    public enum TargetType
    {
        Tile,
        Lane,
        Row,
        Quadrant,
        Ring,
        Board,
        Invalid
    }

    public TargetType type = TargetType.Tile;
    public int range = 4;
    
    public string cardName = "";
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


