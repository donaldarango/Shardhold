using UnityEngine;

public class UIManager : MonoBehaviour
{
    public delegate void TurnTimerPausedHandler(bool isTimerPaused);
    public static event TurnTimerPausedHandler TurnTimerPaused;

    public delegate void TurnTimerButtonToggledHandler(bool enabled);
    public static event TurnTimerButtonToggledHandler TurnTimerButtonToggled;

    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

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

    public void EnableTimerButton()
    {
        TurnTimerButtonToggled?.Invoke(true);
    }

    public void DisableTimerButton()
    {
        TurnTimerButtonToggled?.Invoke(false);
    }

    public void PauseTurnTimer()
    {
        TurnTimerPaused?.Invoke(true);
    }

    public void ResumeTurnTimer()
    {
        TurnTimerPaused?.Invoke(false);
    }
}
