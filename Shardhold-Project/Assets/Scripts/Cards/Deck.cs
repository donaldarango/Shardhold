using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public GameObject cardPrefab;
    public List<Transform> cardPositions;

    public TMP_Text drawPileNum;
    //this will be used for converting int into a Card
    public List<Card> cardLookup = new List<Card>();

    //stores what has been unlocked for the player; is not the actual deck necessarily
    //each card is an index value; the value at that index is the number of copies of that card unlocked
    //{4, 0, 1} means that card 0 has 4 copies, card 1 has not been unlocked, and card 2 has only 1 available copy
    List<int> cardsUnlocked = new List<int>();

    //stores the cards in the current draw pile/deck
    //>>>>>>>>>>NO LONGER TRUE: the index position in this list determines order of draw(? - undecided)
    //STILL TRUE: the values indicate the card, which correlates to the index in cardsUnlocked
    public List<int> drawPile = new List<int>();

    //uses same rules as drawPile
    public List<int> discardPile = new List<int>();
    public bool[] occupiedSlots;
    public int handCapacity = 3;

    //uses same rules as drawPile
    public Card[] hand;
    public GameObject[] UIHand;
    private int cardsInHand = 0;

    void Update()
    {
        drawPileNum.text = drawPile.Count().ToString(); 
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
        //check that draw pile is not empty
        if (drawPile.Count() <= 0)
        {
            if (discardPile.Count() > 0)
            {
                SwapDrawAndDiscard();
                if (CustomDebug.DeckDebugging())
                {
                    Debug.Log("Draw pile was empty, which should not happen. Thankfully, the discard pile was not empty, so draw pile has been filled and a card can be drawn.");
                }
            }
            else
            {
                if (CustomDebug.DeckDebugging())
                {
                    Debug.LogError("Attempting to draw card when both the draw and discard pile are empty.");
                }
                return;
            }
        }

        //check that hand is not full
        if (CountCardsInHand() >= hand.Length)
        {
            if (CustomDebug.DeckDebugging())
            {
                Debug.LogError("Attemping to draw a card while the hand is already full with " + CountCardsInHand() + " cards. Cancelling Draw operation.");
            }
            return;
        }

        //choose a card from the draw pile
        int choice = CustomMath.RandomInt(0, drawPile.Count-1);

        //find an open slot
        int openSlot = FindFirstOpenUISlot();

        //add it to hand (non-UI)
        hand[openSlot] = cardLookup[drawPile[choice]];
        cardsInHand++;
        Debug.Log("Card drawn: " + cardLookup[drawPile[choice]].cardName);
        if (CustomDebug.DeckDebugging())
        {
            Debug.Log("There are now " + CountCardsInHand() + " cards in the hand after drawing one.");
        }

        //add it to hand (UI)
        
        Debug.Log("Open slot: " + openSlot);
        Transform cardUISlot = cardPositions[openSlot];         //get the open slot's transform component
        occupiedSlots[openSlot] = true;                         //mark slot as occupied
        UIHand[openSlot] = CreateCardUI(cardLookup[drawPile[choice]], cardUISlot); //instantiates the UI for the card and adds the UI card to the list of UI cards

        //remove from draw pile
        drawPile.RemoveAt(choice);
        Debug.Log(" Cards left in drawpile: " + drawPile.Count);

        //ensure non-empty draw pile
        if(drawPile.Count <= 0)
        {
            SwapDrawAndDiscard();
        }
    }
    
    //finding an open slot/pos in card UI 
    public int FindFirstOpenUISlot(){
        int openSlot = -1;
        for (int i = 0; i < occupiedSlots.Length; i++) {
            if (occupiedSlots[i] == false) {
                openSlot = i;
                return openSlot;
            }
        }
        return openSlot;
    }

    //finding an occupied slot/pos in card UI 
    public int FindFirstOccupiedUISlot()
    {
        for (int i = 0; i < occupiedSlots.Length; i++)
        {
            if (occupiedSlots[i] == true)
            {
                return i;
            }
        }
        return -1;
    }

    public void DiscardTopmostCard()
    {
        DiscardCard(FindFirstOccupiedUISlot());
    }

    /// <summary>
    /// remove a card from the hand based on its position in the hand list and send it to the discard pile
    /// </summary>
    /// <param name="handPosition">Which card to discard; refers to position in hand, *NOT* the card as an int representation</param>
    public void DiscardCard(int handPosition)
    {
        //check that the hand is not empty and that the handPosition is valid
        if (CountCardsInHand() <=0)
        {
            if (CustomDebug.DeckDebugging())
            {
                Debug.LogError("Hand is empty; cannot discard.");
            }
            return;
        }
        if (handPosition >= hand.Length)
        {
            if (CustomDebug.DeckDebugging())
            {
                Debug.LogError("Attempting to discard above the hand's maximum index of " + (hand.Length - 1) + " with handPosition of " + handPosition);
            }
            return;
        }
        if (handPosition < 0)
        {
            if (CustomDebug.DeckDebugging())
            {
                Debug.LogError("Attempting to discard with a negative handPosition: " + handPosition);
            }
            return;
        }
        if (occupiedSlots[handPosition] == false)
        {
            if (CustomDebug.DeckDebugging())
            {
                Debug.LogError("Attempting to discard from an unoccupied slot: " + handPosition);
            }
            return;
        }

        //send to discard pile
        discardPile.Add(hand[handPosition].GetId());
        Debug.Log("Card discarded: " + hand[handPosition].cardName);

        //free up occupied slot
        occupiedSlots[handPosition] = false;

        //remove from hand (UI)
        Destroy(UIHand[handPosition]);  //delete the UI card
        UIHand[handPosition] = null;  //remove from the list

        //remove from hand (non-UI)
        hand[handPosition] = null;    //since hand references the cards, we DO NOT delete the cards with the current implementation, just remove the reference to it by deleting the entry in the hand list
        cardsInHand--;
        if (CustomDebug.DeckDebugging())
        {
            Debug.Log("There are now " + CountCardsInHand() + " cards in the hand after discarding one.");
        }
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

    public GameObject CreateCardUI(Card card, Transform position) {
        GameObject cardObject = Instantiate(cardPrefab, position);
        CardUI cardUI = cardObject.GetComponent<CardUI>();
        if (cardUI != null)
        {
            cardUI.initializeCardUI(card);
        }
        return cardObject;
    }

    public void ClearHand()
    {
        for (int i = 0; i < hand.Length; i++)
        {
            if (occupiedSlots[i])
            {
                DiscardCard(i);
            }
        }
    }
    #region Pile to Unlocked Cards Comparisons
    
    /// <summary>
    /// How many of a given card are unlocked but not yet in the pile
    /// ASSUMES THAT DISCARD AND HAND ARE EMPTY
    /// </summary>
    /// <param name="card">the int representation of the card to check</param>
    /// <returns>how many of that card are still available to add to draw pile</returns>
    public int InstancesOfCardRemaining(int card)
    {
        int numInPile = 0;
        for (int i = 0; i < drawPile.Count-1; i++)
        {
            if (drawPile[i] == card)
            {
                numInPile++;
            }
        }

        return cardsUnlocked[card] - numInPile;
    }

    /// <summary>
    /// Check if the given draw pile is possible based on what cards are unlocked
    /// ASSUMES THAT DISCARD AND HAND ARE EMPTY
    /// </summary>
    /// <returns>true: pile is valid; false: pile is invalid, containing some cards that are not unlocked OR more cards of a kind than are unlcoked of that kind</returns>
    public bool ValidatePile()
    {
        //slow version; does a lot of looping that could be avoided
        //TODO make a more efficient version if necessary
        for (int i = 0;i < drawPile.Count-1; i++)
        {
            if (InstancesOfCardRemaining(drawPile[i]) < 0)
            {
                return false;
            }
        }
        return true;
    }

    #endregion

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
        //int cardInt = hand[handPosition];   //get the int representation of the card at the given position in the hand
        //Card card = cardLookup[cardInt];    //turn the int representation into an actual Card object
        //return card;
        return hand[handPosition];
    }

    /// <summary>
    /// returns the number of cards in the hand
    /// </summary>
    /// <returns></returns>
    public int CountCardsInHand()
    {
        return cardsInHand;
    }

    #endregion
}
