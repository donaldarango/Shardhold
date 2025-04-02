using System.Collections.Generic;
using UnityEngine;

public class CardPlayTutorial : Tutorial
{
    public int cardIndexToPlay;
    public int cardIdToPlay;
    public int tileRing;
    public int tileLane;

    private void OnEnable()
    {
        Card.PlayCard += OnCardPlayed;
    }

    private void OnDisable()
    {
        Card.PlayCard -= OnCardPlayed;
    }

    // enables card at card index and waits for card to be played
    public override void TutorialStart()
    {
        TutorialUIManager.Instance.HideNextButton();
        Deck.Instance.EnableCardInteraction(cardIndexToPlay);
    }

    public void OnCardPlayed(HashSet<(int, int)> tiles, Card card)
    {
        int currentOrder = TutorialManager.Instance.GetCurrentOrder();
        if (order == currentOrder)
        {
            foreach (var tile in tiles)
            {
                if (tile.Item1 == tileRing && tile.Item2 == tileLane && card.id == cardIdToPlay)
                {
                    TutorialManager.Instance.CompletedTutorial();
                }
            }
        }
    }

}
