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
    [SerializeField]Card card;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void initializeCardUI (Card card) {
        cardImage.sprite = card.cardImage;
        cardName.text = card.cardName;
        cardDescription.text = card.description;
        if (card is Spell) {
            hp.text = "0";
            cardDescription.text += " This card deals damage in a " + card.targetType + ".";
        }
        range.text = card.range.ToString();
        damage.text = card.damage.ToString();
    }
}
