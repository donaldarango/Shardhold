using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TurnTimer : MonoBehaviour
{
    [SerializeField]
    public static float time = 10.0f;
    public float resetTime; 
    public Gradient gradient;
    public Image RightBar;
    public Image LeftBar;

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
    }

    private void OnDisable()
    {
        GameManager.PlayerTurnEnd -= OnEnemyTurnStart;
    }

    private void Start()
    {
        ResetTimerValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.playerTurn == true && time > 0) {
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
            TurnTimerPressed?.Invoke();
        }
    }

    public void TimerButton()
    {
        TurnTimerPressed?.Invoke();
    }

    public void ResetTimerValues()
    {
        time = resetTime;
        sliderLeft.maxValue = resetTime;
        sliderRight.maxValue = resetTime;
        sliderLeft.value = resetTime;
        sliderRight.value = resetTime;
    }

    public void OnEnemyTurnStart()
    {
        ResetTimerValues();
    }
}
