using System;
using System.Collections.Generic;
using UnityEngine;



public class CardPlayTutorial : Tutorial
{
    public int cardIndexToPlay;
    public int cardIdToPlay;

    private void OnEnable()
    {
        Card.PlayCard += OnCardPlayed;
        AllyUnit.PlayAllyUnit += OnAllyUnitPlayed;
    }

    private void OnDisable()
    {
        Card.PlayCard -= OnCardPlayed;
        AllyUnit.PlayAllyUnit -= OnAllyUnitPlayed;
    }

    // enables card at card index and waits for card to be played
    public override void TutorialStart()
    {
        base.TutorialStart();
        TutorialUIManager.Instance.HideNextButton();
        Deck.Instance.EnableCardInteraction(cardIndexToPlay);
    }

    public void OnCardPlayed(HashSet<(int, int)> tiles, Card card)
    {
        int currentOrder = TutorialManager.Instance.GetCurrentOrder();
        if (order == currentOrder && card.id == cardIdToPlay)
        {
            foreach (var tile in tiles)
            {
                if (tileSet.Contains(tile))
                {
                    TutorialManager.Instance.CompletedTutorial();
                    return;
                }
            }
        }
    }

    public void OnAllyUnitPlayed(HashSet<(int, int)> tiles, AllyUnit unit)
    {
        int currentOrder = TutorialManager.Instance.GetCurrentOrder();
        if (order == currentOrder && unit.stats.id == cardIdToPlay)
        {
            foreach (var tile in tiles)
            {
                if (tileSet.Contains(tile))
                {
                    TutorialManager.Instance.CompletedTutorial();
                    return;
                }
            }
        }
    }

}
