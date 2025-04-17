using UnityEngine;
using TMPro;

public class Base : MonoBehaviour
{
    public static Base Instance { get { return _instance; } }
    private static Base _instance;

    public int maxHealth = 20;
    [SerializeField] private int currentHealth;
    public GameObject gameOverScreen;
    public TMP_Text gameOverText;
    public TMP_Text restartText;
    [SerializeField] private TMP_Text baseHP;

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
        if(gameOverText != null)
        {
            gameOverText.text = "GAME OVER";
        }
        if (restartText != null)
        {
            restartText.text = "TRY AGAIN";
        }
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
}
