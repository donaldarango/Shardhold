using System;
using System.Collections.Generic;
using UnityEngine;



public class CardPlayTutorial : Tutorial
{
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

    public int cardIndexToPlay;
    public int cardIdToPlay;
    public List<PlayableTile> playableTiles = new List<PlayableTile>();

    private HashSet<(int,int)> tileSet = new HashSet<(int,int)>();

    private void OnEnable()
    {
        Card.PlayCard += OnCardPlayed;
    }

    private void OnDisable()
    {
        Card.PlayCard -= OnCardPlayed;
    }

    private void Start()
    {
        foreach (var tile in playableTiles)
        {
            tileSet.Add((tile.ringNumber, tile.laneNumber));
        }
    }

    // enables card at card index and waits for card to be played
    public override void TutorialStart()
    {
        TutorialUIManager.Instance.HideNextButton();
        Deck.Instance.EnableCardInteraction(cardIndexToPlay);

        MapGenerator.Instance.StartFlashingTiles(tileSet);
    }

    public void OnCardPlayed(HashSet<(int, int)> tiles, Card card)
    {
        int currentOrder = TutorialManager.Instance.GetCurrentOrder();
        if (order == currentOrder)
        {
            foreach (var tile in tiles)
            {
                if (tileSet.Contains(tile) && card.id == cardIdToPlay)
                {
                    MapGenerator.Instance.StopFlashingTiles(tileSet);
                    TutorialManager.Instance.CompletedTutorial();
                    return;
                }
            }
        }
    }

}
