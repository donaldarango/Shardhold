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
        base.TutorialStart();
        TutorialUIManager.Instance.ShowNextButton();
    }

    public void OnNextButtonPressed(int currentOrder)
    {
        if (order == currentOrder)
        {
            Debug.Log($"Text tutorial completed: {order}");
            completed = true;
            TutorialManager.Instance.CompletedTutorial();
        }
    }
}
