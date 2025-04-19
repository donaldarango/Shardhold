using UnityEngine;
using TMPro;
using DG.Tweening;

public class Base : MonoBehaviour
{
    public static Base Instance { get { return _instance; } }
    private static Base _instance;

    public int maxHealth = 20;
    [SerializeField] private int currentHealth;
    public GameObject gameOverScreen;
    [SerializeField] private TMP_Text baseHP;
    [SerializeField] public GameObject damageIndicatorPrefab;
    public SpriteRenderer spriteRenderer;

    bool setupComplete = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            throw new System.Exception("An instance of this singleton already exists.");
        }
        else
        {
            _instance = this;
        }
    }

    private void OnEnable()
    {
        EnemyUnit.DamageBase += OnTakeDamage;
    }

    private void OnDisable()
    {
        EnemyUnit.DamageBase -= OnTakeDamage;
    }

    void Start()
    {
        Setup();
        if (gameOverScreen!= null) 
        {
            gameOverScreen.SetActive(false);
            if (Time.timeScale == 0) {
                Time.timeScale = 1;
            }
        }
    }

    public void Setup()
    {
        if (!setupComplete)
        {
            currentHealth = maxHealth;
            baseHP.text = currentHealth + "/" + maxHealth;
            if(GameManager.Instance.baseStartHealth != -1)
            {
                currentHealth = GameManager.Instance.baseStartHealth;
            }
            if (CustomDebug.Debugging(CustomDebug.DebuggingType.Normal))
            {
                Debug.Log("Base setup ran.");
            }
        }
        setupComplete = true;
    }

    public void OnTakeDamage(int amount)
    {
        currentHealth -= amount;

        ShowDamageIndicator(amount, false);
        SpriteDamageAnimation();

        Debug.Log("Base HP: " + currentHealth);
        if (baseHP)
        {
            baseHP.text = currentHealth + "/" + maxHealth;
        }
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        ShowDamageIndicator(amount, true);
        SpriteHealAnimation();

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (baseHP)
        {
            baseHP.text = currentHealth + "/" + maxHealth;
        }
        Debug.Log("Base healed by " + amount + ", now at " + currentHealth);
    }

    void GameOver()
    {
        Debug.Log("Game Over! The base was destroyed.");
        Destroy(gameObject);
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void SetBaseHealth(int health)
    {
        currentHealth = health;
    }

    public int GetBaseHealth()
    {
        return currentHealth;
    }

    public void ShowDamageIndicator(int amount, bool isHealing)
    {
        if (IndicatorUIManager.Instance == null || IndicatorUIManager.Instance.damageCanvas == null)
        {
            Debug.LogError("UIManager or DamageCanvas not set!");
            return;
        }

        Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.7f, 0.7f), 0.0f, 0); // world-space offset to left & slightly up
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + offset);
        GameObject indicator = Instantiate(damageIndicatorPrefab, IndicatorUIManager.Instance.damageCanvas.transform);
        indicator.transform.position = screenPos;

        TMP_Text text = indicator.GetComponent<TMP_Text>();

        // Set text and color
        text.text = (isHealing ? "+ " : "- ") + Mathf.Abs(amount).ToString();
        text.color = isHealing ? Color.green : Color.red;

        float scaleFactor = Mathf.Clamp01((float)amount / 5); // between 0 and 1
        float baseScale = 1f;
        float extraScale = 0.5f;

        indicator.transform.localScale = Vector3.one * (baseScale + scaleFactor * extraScale);
    }

    public void SpriteDamageAnimation()
    {
        Sequence mySequence = DOTween.Sequence();
        Color originalColor = spriteRenderer.material.color;
        mySequence.Append(spriteRenderer.material.DOColor(Color.red, 0.2f));
        mySequence.Append(spriteRenderer.material.DOColor(originalColor, 0.2f));
    }

    public void SpriteHealAnimation()
    {
        Sequence mySequence = DOTween.Sequence();
        Color originalColor = spriteRenderer.material.color;
        mySequence.Append(spriteRenderer.material.DOColor(Color.green, 0.2f));
        mySequence.Append(spriteRenderer.material.DOColor(originalColor, 0.2f));
    }
}
