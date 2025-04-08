using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CustomDebug;

public class Deck : MonoBehaviour
{
    public GameObject cardPrefab;
    public List<Transform> cardPositions;

    public TMP_Text drawPileNum;
    public static Deck Instance { get; private set; }

    //this will be used for converting int into a Card
    public List<ScriptableObject> cardLookup = new List<ScriptableObject>();

    //stores what has been unlocked for the player; is not the actual deck necessarily
    //each card is an index value; the value at that index is the number of copies of that card unlocked
    //{4, 0, 1} means that card 0 has 4 copies, card 1 has not been unlocked, and card 2 has only 1 available copy
    public List<int> cardsUnlocked = new List<int>();

    //stores the cards in the current draw pile/deck
    //>>>>>>>>>>NO LONGER TRUE: the index position in this list determines order of draw(? - undecided)
    //STILL TRUE: the values indicate the card, which correlates to the index in cardsUnlocked
    public List<int> drawPile = new List<int>();

    //uses same rules as drawPile
    public List<int> discardPile = new List<int>();
    public bool[] occupiedSlots;

    CardUI selectedCardUI;

    //uses same rules as drawPile
    public ScriptableObject[] hand;
    public GameObject[] UIHand;
    private int cardsInHand = 0;

    private bool deckDisabled;
    public enum DrawChoiceMode
    {
        Random,
        InOrder
    }
    public DrawChoiceMode drawChoiceMode;

    void Update()
    {
        drawPileNum.text = drawPile.Count().ToString(); 
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (DeckDebugging(DebuggingType.Warnings))
            {
                Debug.Log("Multiple deck instances detected.");
            }
        }
    }

    #region Public Methods (not including Gets and Sets)

    /// <summary>
    /// Call this at the start of the player's turn
    /// </summary>
    public void NextTurn()
    {
        DrawCardsUntilFull();

        foreach(GameObject obj in UIHand)
        {
            AllyUnit unit = obj.GetComponent<AllyUnit>();
            if(unit != null)
            {
                unit.currentAttacks = unit.stats.attacks;
                Transform background = unit.transform.Find("CardColor");
                background.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            }
        }
    }

    public void DisableDeckInteraction()
    {
        deckDisabled = true;
        foreach(GameObject obj in UIHand)
        {
            if (!obj)
                return;

            Button button = obj.GetComponent<Button>();
            button.enabled = false;
        }
    }

    public void EnableDeckInteraction()
    {
        deckDisabled = false;
        foreach (GameObject obj in UIHand)
        {
            if (!obj)
                return;

            Button button = obj.GetComponent<Button>();
            button.enabled = true;
        }
    }

    public void DisableCardInteraction(int index)
    {
        GameObject obj = UIHand[index];
        if (obj != null)
        {
            Button button = obj.GetComponent<Button>();
            button.enabled = false;
        }
        else
        {
            Debug.Log($"Card not found at index {index} when trying to disable interaction");
        }
    }

    public void EnableCardInteraction(int index)
    {
        GameObject obj = UIHand[index];
        if (obj != null)
        {
            Button button = obj.GetComponent<Button>();
            button.enabled = true;
        }
        else
        {
            Debug.Log($"Card not found at index {index} when trying to enable interaction");
        }
    }

    public void HandleCardSelection(CardUI newCard)
    {
        // deselect old card
        if (selectedCardUI != null)
        {
            selectedCardUI.DeselectCardAnimation();
        }

        // select new card
        selectedCardUI = newCard;
        selectedCardUI.SelectCardAnimation();
    }


    #endregion

    public ScriptableObject CreateCard(int cardID)
    {
        //find an open slot
        int openSlot = FindFirstOpenUISlot();

        //add it to hand (non-UI)
        ScriptableObject intermediate = cardLookup[cardID];

        hand[openSlot] = intermediate;
        cardsInHand++;
        if (intermediate is Card)
        {
            Debug.Log("Card drawn: " + ((Card)intermediate).cardName);
        }
        else
        {
            Debug.Log("Card drawn: " + ((AllyUnitStats)intermediate).cardName);
        }
        if (CustomDebug.DeckDebugging(DebuggingType.ErrorOnly))
        {
            //Debug.Log("There are now " + CountCardsInHand() + " cards in the hand after drawing one.");
        }

        //add it to hand (UI)

        //Debug.Log("Open slot: " + openSlot);
        Transform cardUISlot = cardPositions[openSlot];         //get the open slot's transform component
        occupiedSlots[openSlot] = true;                         //mark slot as occupied
        UIHand[openSlot] = CreateCardUI(cardLookup[cardID], cardUISlot); //instantiates the UI for the card and adds the UI card to the list of UI cards
        UIHand[openSlot].GetComponent<CardUI>().cardIndex = openSlot;
        return intermediate;
    }

    public void DeleteCard(int handPosition)
    {
        //free up occupied slot
        occupiedSlots[handPosition] = false;

        //remove from hand (UI)
        Destroy(UIHand[handPosition]);  //delete the UI card
        UIHand[handPosition] = null;  //remove from the list

        //remove from hand (non-UI)
        hand[handPosition] = null;    //since hand references the cards, we DO NOT delete the cards with the current implementation, just remove the reference to it by deleting the entry in the hand list
        cardsInHand--;
        if (CustomDebug.DeckDebugging(DebuggingType.ErrorOnly))
        {
            Debug.Log("There are now " + CountCardsInHand() + " cards in the hand after discarding one.");
        }
    }

    public void DeleteAllCardsInHand()
    {
        for (int i = 0; i < hand.Length; i++)
        {
            if (occupiedSlots[i])
            {
                DeleteCard(i);
            }
        }
    }

    /// <summary>
    /// To be used at the beginning of the player's turn to fill their hand
    /// </summary>
    private void DrawCardsUntilFull()
    {
        int safety = 200;
        while (CountCardsInHand() < hand.Length)
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
                /*if (CustomDebug.DeckDebugging(DebuggingType.ErrorOnly))
                {
                    Debug.Log("Draw pile was empty, which should not happen. Thankfully, the discard pile was not empty, so draw pile has been filled and a card can be drawn.");
                }*/
            }
            else
            {
                if (CustomDebug.DeckDebugging(DebuggingType.ErrorOnly))
                {
                    Debug.LogError("Attempting to draw card when both the draw and discard pile are empty.");
                }
                return;
            }
        }

        //check that hand is not full
        if (CountCardsInHand() >= hand.Length)
        {
            if (CustomDebug.DeckDebugging(DebuggingType.ErrorOnly))
            {
                Debug.LogError("Attemping to draw a card while the hand is already full with " + CountCardsInHand() + " cards. Cancelling Draw operation.");
            }
            return;
        }

        //choose a card from the draw pile
        int choice = 0;

        switch (drawChoiceMode)
        {
            case DrawChoiceMode.Random:
                choice = CustomMath.RandomInt(0, drawPile.Count - 1);
                break;
            case DrawChoiceMode.InOrder:
                choice = 0;
                break;
            default:
                if (CustomDebug.DeckDebugging(DebuggingType.ErrorOnly))
                {
                    Debug.LogError("Unhandled DrawChoiceMode");
                }
                break;
        }


        CreateCard(drawPile[choice]);

        //remove from draw pile
        drawPile.RemoveAt(choice);
        //Debug.Log(" Cards left in drawpile: " + drawPile.Count);

        //ensure non-empty draw pile; MOVED TO START OF DRAW
        /*if (drawPile.Count <= 0)
        {
            SwapDrawAndDiscard();
        }*/
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
            if (CustomDebug.DeckDebugging(DebuggingType.ErrorOnly))
            {
                Debug.LogError("Hand is empty; cannot discard.");
            }
            return;
        }
        if (handPosition >= hand.Length)
        {
            if (CustomDebug.DeckDebugging(DebuggingType.ErrorOnly))
            {
                Debug.LogError("Attempting to discard above the hand's maximum index of " + (hand.Length - 1) + " with handPosition of " + handPosition);
            }
            return;
        }
        if (handPosition < 0)
        {
            if (CustomDebug.DeckDebugging(DebuggingType.ErrorOnly))
            {
                Debug.LogError("Attempting to discard with a negative handPosition: " + handPosition);
            }
            return;
        }
        if (occupiedSlots[handPosition] == false)
        {
            if (CustomDebug.DeckDebugging(DebuggingType.ErrorOnly))
            {
                Debug.LogError("Attempting to discard from an unoccupied slot: " + handPosition);
            }
            return;
        }

        //send to discard pile
        ScriptableObject intermediate = hand[handPosition];
        if (intermediate is Card)
        {
            discardPile.Add(((Card)intermediate).GetId());
            Debug.Log("Card discarded: " + ((Card)intermediate).cardName);
        }
        else
        {
            discardPile.Add(((AllyUnitStats)intermediate).GetId());
            Debug.Log("Card discarded: " + ((AllyUnitStats)intermediate).cardName);
        }

        DeleteCard(handPosition);
    }

    /// <summary>
    /// use this when draw pile is empty to send the discard pile to the draw pile (and vice versa)
    /// </summary>
    private void SwapDrawAndDiscard()
    {
        List<int> temp = drawPile;
        drawPile = discardPile;
        discardPile = temp;
    }

    public GameObject CreateCardUI(ScriptableObject card, Transform position) {
        GameObject cardObject = Instantiate(cardPrefab, position);
        if(card is AllyUnitStats)
        {
            AllyUnit unit = cardObject.AddComponent<AllyUnit>();
            unit.stats = (AllyUnitStats)card;
        }

        CardUI cardUI = cardObject.GetComponent<CardUI>();
        if (cardUI != null)
        {
            cardUI.initializeCardUI(card);
            if (deckDisabled)
            {
                cardUI.DisableButton();
            }
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
    /*public List<Card> GetCardsInHand()
    {
        List <Card> cards = new List<Card>();
        for (int i = 0; i < CountCardsInHand(); i++)
        {
            cards.Add(cardLookup[i]);
        }
        return cards;
    }*/

    /// <summary>
    /// get the Card that is at a certain position in the hand
    /// </summary>
    /// <param name="handPosition">The position in the hand to investigate</param>
    /// <returns></returns>
    public ScriptableObject GetCardInHandPos(int handPosition)
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

    public void SetCardsInHand(int amt)
    {
        if (CustomDebug.DeckDebugging(DebuggingType.Normal))
        {
            Debug.Log("External setting of cardsInHand variable occurred. This should only happen when loading a saved game.");
        }
        cardsInHand = amt;
    }

    /// <summary>
    /// returns the number of cards in the draw pile
    /// </summary>
    /// <returns></returns>
    public int CountCardsInDrawPile()
    {
        return drawPile.Count;
    }
    // returns the number of cards in the discard pile
    /// </summary>
    /// <returns></returns>
    public int CountCardsInDiscardPile()
    {
        return discardPile.Count;
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        TileActorManager.EndEnemyTurn += NextTurn;
    }

    private void OnDisable()
    {
        TileActorManager.EndEnemyTurn -= NextTurn;
    }
    #endregion
}
