using UnityEngine;

public class TimerPressedTutorial : Tutorial
{
    private void OnEnable()
    {
        GameManager.PlayerTurnEnd += OnPlayerTurnEnd;
    }

    private void OnDisable()
    {
        GameManager.PlayerTurnEnd -= OnPlayerTurnEnd;

    }

    public override void TutorialStart()
    {
        TutorialUIManager.Instance.HideNextButton();
    }

    public void OnPlayerTurnEnd()
    {
        int currentOrder = TutorialManager.Instance.GetCurrentOrder();
        if (order == currentOrder)
        {
            TutorialManager.Instance.CompletedTutorial();
        }
    }
}
