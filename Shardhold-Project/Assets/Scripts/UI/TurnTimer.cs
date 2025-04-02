using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TurnTimer : MonoBehaviour
{
    [SerializeField]
    public static float time = 10.0f;
    public bool isTimerPaused = false;
    public bool isTimerButtonEnabled;
    public float resetTime; 
    public Gradient gradient;
    public Image RightBar;
    public Image LeftBar;

    public Button timerButton;
    public TMP_Text timeText;
    public Slider sliderRight;
    public Slider sliderLeft;
    //Consider an audio clip that plays in the last few seconds like a league of legends champ select countdown
    //Consider a lock in/turn switch sound when time hits 0

    public delegate void TurnTimerPressedHandler();
    public static event TurnTimerPressedHandler TurnTimerPressed;

    private void OnEnable()
    {
        GameManager.PlayerTurnEnd += OnEnemyTurnStart;
        UIManager.TurnTimerButtonToggled += OnTurnTimerButtonToggled;
        UIManager.TurnTimerPaused += OnTurnTimerPaused;
    }

    private void OnDisable()
    {
        GameManager.PlayerTurnEnd -= OnEnemyTurnStart;
        UIManager.TurnTimerButtonToggled -= OnTurnTimerButtonToggled;
        UIManager.TurnTimerPaused -= OnTurnTimerPaused;
    }

    private void Start()
    {
        ResetTimerValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.playerTurn == true && time > 0 && !isTimerPaused) {
            string timerInt = time.ToString("0");
            timeText.text = timerInt;
            time -= Time.deltaTime;
            sliderRight.value -= Time.deltaTime;
            sliderLeft.value -= Time.deltaTime;
            float timeNormalized = time/resetTime;
            RightBar.color = gradient.Evaluate(timeNormalized);
            LeftBar.color = gradient.Evaluate(timeNormalized);

        }
        if (time <= 0 && GameManager.Instance.playerTurn == true) {
            TimerButton();
        }
    }

    public void TimerButton()
    {
        TurnTimerPressed?.Invoke();
        ResetTimerValues();
    }

    public void ResetTimerValues()
    {
        time = resetTime;
        timeText.text = resetTime.ToString();
        RightBar.color = gradient.Evaluate(resetTime);
        LeftBar.color = gradient.Evaluate(resetTime);
        sliderLeft.maxValue = resetTime;
        sliderRight.maxValue = resetTime;
        sliderLeft.value = resetTime;
        sliderRight.value = resetTime;
    }

    public void OnEnemyTurnStart()
    {
        ResetTimerValues();
    }

    public void OnTurnTimerButtonToggled(bool toggled)
    {
        timerButton.enabled = toggled;
    }

    public void OnTurnTimerPaused(bool timerPaused)
    {
        isTimerPaused = timerPaused;
    }
}
