using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using static CardPlayTutorial;

[Serializable]
public enum TutorialSetting
{
    pausesTimer,
    resumesTimer,
    disablesTimer,
    enablesTimer,
    disablesCards,
    enablesCards,
}

[Serializable]
public struct PlayableTile
{
    public int ringNumber;
    public int laneNumber;

    public PlayableTile(int ringNumber, int laneNumber)
    {
        this.ringNumber = ringNumber;
        this.laneNumber = laneNumber;
    }
}

[Serializable]
public enum TutorialUIPosition
{
    Default,
    Card,
}

public class Tutorial : MonoBehaviour
{
    public bool completed = false;
    public int order;
    public string title;
    [TextArea(5,10)]
    public string explanation = "";
    public TutorialSetting[] settings;
    public TutorialUIPosition position;

    public List<PlayableTile> highlightTiles = new List<PlayableTile>();

    protected HashSet<(int, int)> tileSet = new HashSet<(int, int)>();

    private void Awake()
    {
        TutorialManager.Instance.AddTutorialToList(this);
    }

    public void StopTileHighlight()
    {
        if (tileSet.Count > 0)
            MapGenerator.Instance.StopFlashingTiles(tileSet);
    }

    public virtual void TutorialStart() 
    {
        foreach (var tile in highlightTiles)
        {
            tileSet.Add((tile.ringNumber, tile.laneNumber));
        }
        if (tileSet.Count > 0)
            MapGenerator.Instance.StartFlashingTiles(tileSet);
    }

    public virtual void CheckIfHappening() { }
}
