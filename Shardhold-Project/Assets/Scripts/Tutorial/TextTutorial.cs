using UnityEngine;

public class TextTutorial : Tutorial
{
    private void OnEnable()
    {
        TutorialUIManager.NextButtonPressed += OnNextButtonPressed;
    }

    private void OnDisable()
    {
        TutorialUIManager.NextButtonPressed -= OnNextButtonPressed;
    }

    public override void TutorialStart()
    {
        TutorialUIManager.Instance.ShowNextButton();
    }

    public void OnNextButtonPressed()
    {
        //Debug.Log($"OnNextButtonPressed: Tutorial {order}");
        int currentOrder = TutorialManager.Instance.GetCurrentOrder();
        if (order == currentOrder)
        {
            completed = true;
            Debug.Log($"Completed tutorial {order}");
            TutorialManager.Instance.CompletedTutorial();
        }
    }
}
