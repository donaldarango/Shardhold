using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileActorManager : MonoBehaviour
{
    [Serializable]
    public struct EnemySpawnData
    {
        [SerializeField] private int spawnTurn;
        [SerializeField] private int laneNumber;
        [SerializeField] private EnemyUnit enemyUnit;
    }

    private static TileActorManager _instance;
    [SerializeField] private List<TileActor> tileActors = new List<TileActor>();
    [SerializeField] private List<GameObject> testPrefabs = new List<GameObject>(); // For test purposes

    public static TileActorManager Instance { get { return _instance; } }
    

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            throw new System.Exception("An instance of this singleton already exists.");
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public TileActor GetTileActor(int index)
    {
        return tileActors[index];
    }

    public GameObject GetTAPrefab(int index) // For test purposes
    {
        return testPrefabs[index];
    }
}
