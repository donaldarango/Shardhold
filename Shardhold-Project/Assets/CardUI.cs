using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField]private UnityEngine.UI.Image cardImage;
    [SerializeField]private TMP_Text cardName;
    [SerializeField]private TMP_Text cardDescription;
    [SerializeField]private TMP_Text hp;
    [SerializeField]private TMP_Text range;
    [SerializeField]private TMP_Text damage;
    [SerializeReference]Card card_;
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
    public void initializeCardUI (Card card) {
        //finds the card color component of the prefab
        card_ = card;
        Transform background = transform.Find("CardColor");
        cardImage.sprite = card.cardImage;
        cardName.text = card.cardName;
        cardDescription.text = card.description;
        if (card is Spell) {
            hp.text = "0";
            cardDescription.text += "\nThis card deals damage in a " + card.targetType + ".";
            background.GetComponent<UnityEngine.UI.Image>().color = Color.green;
        }
        if (card is Placer) {
            hp.text = ((Placer)card).stats.maxHealth.ToString();
            // background.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        }
        range.text = card.range.ToString();
        damage.text = card.damage.ToString();
    }
    public void OnCardSelected(){
        MapGenerator mapGenerator = FindFirstObjectByType<MapGenerator>();
        Debug.Log("Card selected/clicked: " + cardName.text);
        mapGenerator.SelectCard(card_);
    }
}
