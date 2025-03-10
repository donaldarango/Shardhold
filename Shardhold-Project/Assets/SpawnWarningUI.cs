using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpawnWarningUI : MonoBehaviour
{

    public List<Image> warningImages;

    private void OnEnable()
    {
        MapManager.AddEnemyToSpawnTileEvent += turnWarningLabelOn;
        MapManager.RemoveEnemyFromSpawnTileEvent += turnWarningLabelOff;
    }

    private void OnDisable()
    {
        MapManager.AddEnemyToSpawnTileEvent -= turnWarningLabelOn;
        MapManager.RemoveEnemyFromSpawnTileEvent -= turnWarningLabelOff;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var warningImage in warningImages)
        {
            warningImage.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void turnWarningLabelOn(int laneNumber)
    {
        warningImages[laneNumber].enabled = true;
    }

    private void turnWarningLabelOff(int laneNumber)
    {
        warningImages[laneNumber].enabled = false;

    }
}
