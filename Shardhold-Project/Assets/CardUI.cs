using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image cardImage;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardDescription;
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text range;
    [SerializeField] private TMP_Text damage;
    [SerializeField] public int cardIndex;

    [SerializeReference] Card card_;
    [SerializeReference] AllyUnitStats unit_;
    [SerializeField]Button cardButton;

    private void Awake()
    {
        // Get the Button component attached to this object
        cardButton = GetComponent<Button>();

        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnCardSelected);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void updateHealth(int health){
        hp.text = health.ToString();
    }
    public void initializeCardUI (ScriptableObject intermediate) {
        //finds the card color component of the prefab
        if (intermediate is Card)
        {
            Debug.Log("Card drawn: " + ((Card)intermediate).cardName);
            Card card = (Card)intermediate;

            card_ = card;
            Transform background = transform.Find("CardColor");
            cardImage.sprite = card.cardImage;
            cardName.text = card.cardName;
            cardDescription.text = card.description;
            if (card is Spell)
            {
                hp.text = "0";
                cardDescription.text += "\nThis card deals damage in a " + card.targetType + ".";
                background.GetComponent<UnityEngine.UI.Image>().color = Color.green;
            }
            if (card is Placer)
            {
                hp.text = ((Placer)card).stats.maxHealth.ToString();
                // background.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            }
            range.text = card.range.ToString();
            damage.text = card.damage.ToString();
        }
        else
        {
            Debug.Log("Card drawn: " + ((AllyUnitStats)intermediate).cardName);
            AllyUnitStats allystats = (AllyUnitStats)intermediate; ;

            unit_ = allystats;
            Transform background = transform.Find("CardColor");
            background.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            cardImage.sprite = allystats.cardImage;
            cardName.text = allystats.cardName;
            cardDescription.text = allystats.description;
            hp.text = allystats.hp.ToString();
            
            range.text = allystats.range.ToString();
            damage.text = allystats.damage.ToString();
        }

        //card_ = card;
        //Transform background = transform.Find("CardColor");
        //cardImage.sprite = card.cardImage;
        //cardName.text = card.cardName;
        //cardDescription.text = card.description;
        //if (card is Spell) {
        //    hp.text = "0";
        //    cardDescription.text += "\nThis card deals damage in a " + card.targetType + ".";
        //    background.GetComponent<UnityEngine.UI.Image>().color = Color.green;
        //}
        //if (card is Placer) {
        //    hp.text = ((Placer)card).stats.maxHealth.ToString();
        //    // background.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        //}
        //range.text = card.range.ToString();
        //damage.text = card.damage.ToString();
    }
    public void OnCardSelected(){
        MapGenerator mapGenerator = FindFirstObjectByType<MapGenerator>();
        Debug.Log("Card selected/clicked: " + cardName.text);
        Debug.Log("Hand Index: " + cardIndex);
        //mapGenerator.selectedHandIndex = cardIndex;

        if (card_ != null)
        {
            Debug.Log("Card selected/clicked: " + cardName.text);
            Debug.Log("Hand Index: " + cardIndex);
            mapGenerator.selectedHandIndex = cardIndex;
            mapGenerator.SelectCard(card_);
        }
        else if (unit_ != null)
        {
            AllyUnit instance = this.gameObject.GetComponent<AllyUnit>();
            if (instance.currentAttacks > 0)
            {
                Debug.Log("Card selected/clicked: " + cardName.text);
                Debug.Log("Hand Index: " + cardIndex);
                mapGenerator.selectedHandIndex = cardIndex;
                mapGenerator.SelectUnit(instance);
            }
            else
            {
                Debug.Log("ignoring attempt to select exhausted unit");
            }

        }
    }

    public void DisableButton()
    {
        cardButton.enabled = false;
    }

    public void EnableButton()
    {
        cardButton.enabled = true;
    }
}
