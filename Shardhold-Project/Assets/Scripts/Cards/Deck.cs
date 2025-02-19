using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Deck : MonoBehaviour
{


    //stores what has been unlocked for the player; is not the actual deck necessarily
    //each card is an index value; the value at that index is the number of copies of that card unlocked
    //{4, 0, 1} means that card 0 has 4 copies, card 1 has not been unlocked, and card 2 has only 1 available copy
    List<int> cardsUnlocked = new List<int>();

    //stores the cards in the current draw pile/deck
    //the index position in this list determines order of draw(? - undecided)
    //the values indicate the card, which correlates to the index in cardsUnlocked
    List<int> drawPile = new List<int>();

    List<int> discardPile = new List<int>();

    public int handCapacity = 3;

    List<int> hand = new List<int>();


    public int CountCardsInHand()
    {
        return hand.Count;
    }

    public void DrawCardsUntilFull()
    {
        int safety = 200;
        while (CountCardsInHand() < handCapacity)
        {
            if(safety-- < 0) {  break; }
            DrawCard();
        }
    }

    public void DrawCard()
    {
        //TODO
        CustomDebug.RanUnimplementedCode("DrawCard()");
    }
}
