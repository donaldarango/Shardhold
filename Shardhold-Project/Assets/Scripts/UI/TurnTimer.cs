using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;
using System.Threading;
using NUnit.Framework.Internal.Commands;
using Unity.VisualScripting;
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
    public bool playerTurn = false;
    //Consider an audio clip that plays in the last few seconds like a league of legends champ select countdown
    //Consider a lock in/turn switch sound when time hits 0

    public delegate void EnemyTurnHandler();
    public static event EnemyTurnHandler EnemyTurnStart;

    //OnEnable is to /disable is listening for an event so tileactormanager NextRoudn would need to subscribe in here
    private void OnEnable()
    {
        TileActorManager.PlayerTurnStart += StartPlayerTurn;
    }

    private void OnDisable()
    {
        TileActorManager.PlayerTurnStart -= StartPlayerTurn;
    }

    void Start()
    {
        StartPlayerTurn();
    }
    // Update is called once per frame
    void Update()
    {
        if (playerTurn == true && time > 0) {
            string timerInt = time.ToString("0");
            timeText.text = timerInt;
            time -= Time.deltaTime;
            sliderRight.value -= Time.deltaTime;
            sliderLeft.value -= Time.deltaTime;
            float timeNormalized = time/resetTime;
            RightBar.color = gradient.Evaluate(timeNormalized);
            LeftBar.color = gradient.Evaluate(timeNormalized);

        }
        if (time <= 0 && playerTurn == true) {
            EndPlayerTurn();
        }
    }

    public void EndPlayerTurn()
    {
        Debug.Log("Player turn ended");
        playerTurn = false;
        timeText.text = "";
        EnemyTurnStart?.Invoke();

    }
    public void StartPlayerTurn()
    {
        Debug.Log("Player turn started");
        time = resetTime;
        sliderLeft.maxValue = resetTime;
        sliderRight.maxValue = resetTime;
        sliderLeft.value = resetTime;
        sliderRight.value = resetTime;
        playerTurn = true;
    }
}
