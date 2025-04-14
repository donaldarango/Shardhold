using DG.Tweening;
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
    public bool isSelected;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Tween shakeTween;

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

        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void SelectCardAnimation()
    {
        if (isSelected) return;
        isSelected = true;

        shakeTween = DOTween.Sequence()
            .Append(rectTransform.DOAnchorPos(rectTransform.anchoredPosition + new Vector2(2f, 2f), 0.1f).SetEase(Ease.InOutSine))
            .Append(rectTransform.DOAnchorPos(rectTransform.anchoredPosition - new Vector2(4f, 4f), 0.2f).SetEase(Ease.InOutSine))
            .Append(rectTransform.DOAnchorPos(rectTransform.anchoredPosition + new Vector2(2f, 2f), 0.1f).SetEase(Ease.InOutSine))
            .SetLoops(-1);

        rectTransform.DOScale(originalScale * 1.1f, 0.5f).SetEase(Ease.OutQuad);
    }

    public void DeselectCardAnimation()
    {
        if (!isSelected) return;
        isSelected = false;

        shakeTween.Kill(); // Stop shake tween
        rectTransform.DOKill(); // Stop all tweens on this object
        rectTransform.DOScale(originalScale, 0.5f).SetEase(Ease.OutQuad);
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector2.zero; // Reset position
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
            Deck.Instance.HandleCardSelection(this);
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
                Deck.Instance.HandleCardSelection(this);
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
