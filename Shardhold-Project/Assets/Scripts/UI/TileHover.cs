using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class TileHover : MonoBehaviour
{
    [SerializeField]TMP_Text nameDisplay;
    [SerializeField]TMP_Text currentHealthDisplay;
    [SerializeField]TMP_Text attackDisplay;
    [SerializeField]TMP_Text attackRange;
    [SerializeField]GameObject enemySprite;
    public GameObject tileDisplayUI;

    private void OnEnable()
    {
        MapGenerator.HoverTile += OnHover;
    }

    private void OnDisable()
    {
        MapGenerator.HoverTile -= OnHover;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileDisplayUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnHover(TileActor ta)
    {
        if (ta != null)
        {
            tileDisplayUI.SetActive(true);
            ShowStats(ta);
        }
        else {
            tileDisplayUI.SetActive(false);
        }
    }

    void ShowStats(TileActor ta)
    {
        nameDisplay.text = ta.GetActorName();
        currentHealthDisplay.text = "HP    : " +ta.GetCurrentHealth().ToString() + " / " + ta.GetMaxHealth().ToString();
        attackDisplay.text = "ATK   : " + ta.GetAttackDamage().ToString();
        attackRange.text = "RANGE : " +ta.GetAttackRange().ToString();
        enemySprite.GetComponent<Image>().sprite = ta.GetSprite();
    }
}
