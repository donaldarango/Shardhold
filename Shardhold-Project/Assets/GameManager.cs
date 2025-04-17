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

    public int baseStartHealth = -1;    //if not -1, then Base should use this value for the starting health rather than the usual maximum
    public bool showDebugLevelsInMenu = false;

    public enum LevelType
    {
        LevelSettingsFile,
        LevelSaveFile,
        PlayerSaveFile
    }

    public LevelType levelType = LevelType.LevelSettingsFile;

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
        Instance.baseStartHealth = -1;
    }

    public void LoadLevel(string level)
    {
        if (CustomDebug.Debugging(CustomDebug.DebuggingType.Normal))
        {
            Debug.Log($"running LoadLevel: {level}");
        }
        Instance.baseStartHealth = -1;
        Instance.levelType = LevelType.LevelSettingsFile;
        currentLevel = level;
        SceneManager.LoadScene("BaseLevel");
    }

    public void LoadLevelFromSaveFile(string level)
    {
        if (CustomDebug.Debugging(CustomDebug.DebuggingType.Normal))
        {
            Debug.Log($"running LoadLevelFromSaveFile: {level}");
        }
        if(!SaveLoad.saveLoad.CheckIfFileExists($"Level_{level}_Save.json", SaveLoad.SaveType.levelFile))
        {
            if (CustomDebug.Debugging(CustomDebug.DebuggingType.ErrorOnly))
            {
                Debug.Log($"Cannot load non-existant save file: Level_{level}_Save.json");
            }
            return;
        }
        Instance.baseStartHealth = -1;
        Instance.levelType = LevelType.LevelSaveFile;
        currentLevel = level;
        SceneManager.LoadScene("BaseLevel");
        //Base.Instance.Setup();
    }


    public void LoadLastSave()
    {
        if (CustomDebug.Debugging(CustomDebug.DebuggingType.Normal))
        {
            Debug.Log($"running LoadLastSave");
        }
        Instance.baseStartHealth = -1;
        Instance.levelType = LevelType.PlayerSaveFile;
        currentLevel = "";
        SceneManager.LoadScene("BaseLevel");
        //Base.Instance.Setup();
    }

    public void LoadTutorialLevel(int levelNumber)
    {
        Instance.baseStartHealth = -1;
        currentLevel = "Tutorial_" + levelNumber;
        SceneManager.LoadScene("Tutorial Level " + levelNumber);
    }

    public void RestartLevel()
    {
        //Debug.Log("Restarted Level");
        if (Instance.levelType == LevelType.LevelSettingsFile)
        {
            LoadLevel(currentLevel);
        }
        else if(Instance.levelType == LevelType.LevelSaveFile)
        {
            LoadLevelFromSaveFile(currentLevel);
        }
        
    }

    public void LoadMainMenu()
    {
        currentLevel = "";

        if(PauseMenu.isPaused)
        {
            PauseMenu.isPaused = false;
            Time.timeScale = 1f;
        }

        SceneManager.LoadScene("Main-Menu");
    }
    #endregion

    public string GetCurrentLevel()
    {
        return currentLevel;
    }
}


