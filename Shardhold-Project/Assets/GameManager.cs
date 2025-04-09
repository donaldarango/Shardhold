using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public delegate void PlayerTurnEndHandler();
    public static event PlayerTurnEndHandler PlayerTurnEnd;

    //public delegate void ChangeLevelHandler(string level);
    //public static event ChangeLevelHandler ChangeLevel;

    private static string currentLevel = "";
    public string optionalStartLevel = "";
    public bool playerTurn = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            throw new System.Exception("An instance of this GameManager already exists.");
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
            if (optionalStartLevel != string.Empty)
            {
                currentLevel = optionalStartLevel;
            }
        }
    }

    private void OnEnable()
    {
        TileActorManager.EndEnemyTurn += OnStartPlayerTurn;
        TurnTimer.TurnTimerPressed += OnEndPlayerTurn;
    }

    private void OnDisable()
    {
        TileActorManager.EndEnemyTurn -= OnStartPlayerTurn;
        TurnTimer.TurnTimerPressed -= OnEndPlayerTurn;
    }

    private void Start()
    {
        StartGame();
    }

    #region Turn Handling
    public void OnStartPlayerTurn()
    {
        Debug.Log("Player turn started");
        Deck.Instance.EnableDeckInteraction();
        playerTurn = true;
    }

    public void OnEndPlayerTurn()
    {
        Debug.Log("Player turn ended");
        playerTurn = false;
        Deck.Instance.DisableDeckInteraction();
        MapGenerator.Instance.DeselectCard();
        PlayerTurnEnd?.Invoke();
    }

    public void StartGame()
    {
        playerTurn = true;
    }

    #endregion

    #region Scene Loading
    public void LoadByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadLevel(string level)
    {
        currentLevel = level;
        SceneManager.LoadScene("BaseLevel");
    }

    public void LoadTutorialLevel(int levelNumber)
    {
        currentLevel = "Tutorial_" + levelNumber;
        SceneManager.LoadScene("Tutorial Level " + levelNumber);
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevel);
    }

    public void LoadMainMenu()
    {
        currentLevel = "";
        SceneManager.LoadScene("Main-Menu");
    }
    #endregion

    public string GetCurrentLevel()
    {
        return currentLevel;
    }
}


