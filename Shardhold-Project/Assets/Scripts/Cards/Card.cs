using System;
using UnityEngine;
using static MapGenerator;

[Serializable]
abstract public class Card : MonoBehaviour
{
    public int id; //the int representation of the card
    public TargetType type = TargetType.Tile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    #region Gets and Sets
    public int GetId()
    {
        return id;
    }
    #endregion
}
