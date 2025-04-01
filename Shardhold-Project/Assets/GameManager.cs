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
    public bool playerTurn = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            throw new System.Exception("An instance of this GameManager already exists.");
        }
        else
        {
            DontDestroyOnLoad(this);
            _instance = this;
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
        playerTurn = true;
    }

    public void OnEndPlayerTurn()
    {
        Debug.Log("Player turn ended");
        playerTurn = false;
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

    public void LoadTutorialLevel()
    {
        currentLevel = "Tutorial";
        SceneManager.LoadScene("TutorialLevel");
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


