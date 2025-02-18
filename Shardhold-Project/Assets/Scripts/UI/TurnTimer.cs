using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;
public class TurnTimer : MonoBehaviour
{
    [SerializeField]
    public static float time = 10.0f;
    public TMP_Text timeText;
    public bool playerTurn = false;

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
        }
        if (time <= 0) {
            EndPlayerTurn();
        }
    }

    public void EndPlayerTurn()
    {
        Debug.Log("Player turn started");
        playerTurn = false;
        timeText.text = " ";
    }
    public void StartPlayerTurn()
    {
        Debug.Log("Player turn started");
        time = 10.0f;
        playerTurn = true;
    }
}
