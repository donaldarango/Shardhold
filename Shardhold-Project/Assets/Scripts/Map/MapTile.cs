using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    private int circleNumber;
    private int sectorNumber;

    private int terrainType; // PLACEHOLDER TYPE
    private List<GameObject> player_units = new List<GameObject>(); // PLACEHOLDER TYPE
    private List<GameObject> enemy_units = new List<GameObject>(); // PLACEHOLDER TYPE
    private List<GameObject> structures = new List<GameObject>(); // PLACEHOLDER TYPE

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setCircleNumber(int circleNumber)
    {
        this.circleNumber = circleNumber;
    }

    public void setSectorNumber(int circleNumber)
    {
        this.circleNumber = circleNumber;
    }
}
