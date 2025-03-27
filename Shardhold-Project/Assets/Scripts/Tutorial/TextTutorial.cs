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

    public override void Start()
    {
        base.Start();
    }

    public override void  TutorialStart()
    {
        TutorialUIManager.Instance.ShowNextButton();
    }

    public void OnNextButtonPressed()
    {
        int currentOrder = TutorialManager.Instance.GetCurrentOrder();
        if (order == currentOrder)
        {
            TutorialManager.Instance.CompletedTutorial();
        }
    }
}
