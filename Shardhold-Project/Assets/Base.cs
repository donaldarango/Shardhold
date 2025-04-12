using UnityEngine;

public class Base : MonoBehaviour
{
    public static Base Instance { get { return _instance; } }
    private static Base _instance;

    public int maxHealth = 20;
    [SerializeField] private int currentHealth;
    public GameObject gameOverScreen;

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
        currentHealth = maxHealth;
        if (gameOverScreen!= null) 
        {
            gameOverScreen.SetActive(false);
            if (Time.timeScale == 0) {
                Time.timeScale = 1;
            }
        }
    }

    public void OnTakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Base HP: " + currentHealth);

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
}
