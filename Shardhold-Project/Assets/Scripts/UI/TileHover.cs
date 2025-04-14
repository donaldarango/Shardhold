using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;

public class TileHover : MonoBehaviour
{
    [Header("Enemy Stat Displays")]
    [SerializeField]TMP_Text nameDisplay;
    [SerializeField]TMP_Text currentHealthDisplay;
    [SerializeField]TMP_Text attackDisplay;
    [SerializeField]TMP_Text attackRange;
    [SerializeField]GameObject enemySprite;

    [Header("Trap Stat Displays")]
    [SerializeField]TMP_Text trapNameDisplay;
    [SerializeField]TMP_Text trapCurrentHealthDisplay;
    [SerializeField]TMP_Text trapAttackDisplay;
    [SerializeField]GameObject trapEnemySprite;

    [Header("Positions/Transforms")]
    public GameObject tileDisplayUI;
    public GameObject trapDisplayUI;
    public Transform display1;
    public Transform display2;

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
        trapDisplayUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnHover(int ringNumber, int laneNumber)
    {
        if (ringNumber == -1 && ringNumber == -1)
            return;

        TileActor ta = MapManager.Instance.DoesTileContainTileActor(ringNumber, laneNumber);
        TrapUnit trap = MapManager.Instance.GetTile(ringNumber, laneNumber).GetCurrentTrapUnit();
        // Debug.Log(ta.name);
        // Debug.Log(trap.name);
        if (ta != null && trap != null) {
            tileDisplayUI.transform.position = display1.transform.position;
            tileDisplayUI.SetActive(true);
            ShowStats(ta);
            trapDisplayUI.transform.position = display2.transform.position;
            trapDisplayUI.SetActive(true);
            ShowTrapStats(trap);
        }
        else if (ta != null && trap == null)
        {
            tileDisplayUI.transform.position = display1.transform.position;
            tileDisplayUI.SetActive(true);
            ShowStats(ta);
        }
        else if (trap != null && ta == null) {
            trapDisplayUI.transform.position = display1.transform.position;
            trapDisplayUI.SetActive(true);
            ShowTrapStats(trap);
        }
        else {
            tileDisplayUI.SetActive(false);
            trapDisplayUI.SetActive(false);
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
    void ShowTrapStats(TrapUnit trap)
    {
        trapNameDisplay.text = trap.GetActorName();
        trapCurrentHealthDisplay.text = "HP    : " + trap.GetCurrentHealth().ToString() + " / " + trap.GetMaxHealth().ToString();
        trapAttackDisplay.text = "ATK   : " + trap.GetAttackDamage().ToString();
        trapEnemySprite.GetComponent<Image>().sprite = trap.GetSprite();
    }
}
