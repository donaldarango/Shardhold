using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JetBrains.Annotations;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //this will be used for converting int into a Card
    List<Card> cardLookup = new List<Card>();

    //stores what has been unlocked for the player; is not the actual deck necessarily
    //each card is an index value; the value at that index is the number of copies of that card unlocked
    //{4, 0, 1} means that card 0 has 4 copies, card 1 has not been unlocked, and card 2 has only 1 available copy
    List<int> cardsUnlocked = new List<int>();

    //stores the cards in the current draw pile/deck
    //>>>>>>>>>>NO LONGER TRUE: the index position in this list determines order of draw(? - undecided)
    //STILL TRUE: the values indicate the card, which correlates to the index in cardsUnlocked
    List<int> drawPile = new List<int>();

    //uses same rules as drawPile
    List<int> discardPile = new List<int>();

    public int handCapacity = 3;

    //uses same rules as drawPile
    List<int> hand = new List<int>();

    /// <summary>
    /// returns the number of cards in the hand
    /// </summary>
    /// <returns></returns>
    public int CountCardsInHand()
    {
        return hand.Count;
    }

    /// <summary>
    /// To be used at the beginning of the player's turn to fill their hand
    /// </summary>
    public void DrawCardsUntilFull()
    {
        int safety = 200;
        while (CountCardsInHand() < handCapacity)
        {
            if(safety-- < 0) {  break; }
            DrawCard();
        }
    }

    /// <summary>
    /// move a card from the draw pile to the hand
    /// </summary>
    public void DrawCard()
    {
        //choose a card from the draw pile
        int choice = CustomMath.RandomInt(0, drawPile.Count);

        //add it to hand
        hand.Add(drawPile[choice]);

        //remove from draw pile
        drawPile.RemoveAt(choice);

        if(drawPile.Count <= 0)
        {
            SwapDrawAndDiscard();
        }
    }

    /// <summary>
    /// remove a card from the hand based on its position in the hand list and send it to the discard pile
    /// </summary>
    /// <param name="handPosition">Which card to discard; refers to position in hand, *NOT* the card as an int representation</param>
    public void DiscardCard(int handPosition)
    {
        //send to discard pile
        discardPile.Add(hand[handPosition]);

        //remove from hand
        hand.RemoveAt(handPosition);
    }

    /// <summary>
    /// use this when draw pile is empty to send the discard pile to the draw pile (and vice versa)
    /// </summary>
    public void SwapDrawAndDiscard()
    {
        List<int> temp = drawPile;
        drawPile = discardPile;
        discardPile = temp;
    }

    /// <summary>
    /// Check if the given draw pile is possible based on what cards are unlocked; should only be used when hand and discard pile are empty
    /// </summary>
    /// <returns>true: pile is valid; false: pile is invalid, containing some cards that are not unlocked OR more cards of a kind than are unlcoked of that kind</returns>
    public bool ValidatePile()
    {
        //TODO
        if (CustomDebug.DeckDebugging())
        {
            CustomDebug.RanUnimplementedCode("ValidatePile");
        }
        return true;
    }

    #region Gets, Sets, etc.

    /// <summary>
    /// Return a list of card objects that are in the player's hand
    /// </summary>
    /// <returns></returns>
    public List<Card> GetCardsInHand()
    {
        List <Card> cards = new List<Card>();
        for (int i = 0; i < CountCardsInHand(); i++)
        {
            cards.Add(cardLookup[i]);
        }
        return cards;
    }

    /// <summary>
    /// get the Card that is at a certain position in the hand
    /// </summary>
    /// <param name="handPosition">The position in the hand to investigate</param>
    /// <returns></returns>
    public Card GetCardInHandPos(int handPosition)
    {
        int cardInt = hand[handPosition];   //get the int representation of the card at the given position in the hand
        Card card = cardLookup[cardInt];    //turn the int representation into an actual Card object
        return card;
    }

    #endregion
}
