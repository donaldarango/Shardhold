using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

abstract public class Card : MonoBehaviour
{
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

    abstract public void Play();
}


