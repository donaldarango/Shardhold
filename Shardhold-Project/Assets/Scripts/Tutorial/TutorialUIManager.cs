using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{

    private static TutorialUIManager _instance;
    public static TutorialUIManager Instance { get { return _instance; } }

    public delegate void NextButtonPressedHandler();
    public static event NextButtonPressedHandler NextButtonPressed;

    public Canvas tutorialCanvas;
    public TMP_Text titleText;
    public TMP_Text explanationText;
    public Button nextButton;

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

    public void SetTitleText(string text)
    {
        titleText.text = text;
    }

    public void SetExplanationText(string text)
    {
        explanationText.text = text;   
    }

    public void ToggleTutorialUI(bool enabled)
    {
        tutorialCanvas.enabled = enabled;
    }

    public void HideNextButton()
    {
        nextButton.gameObject.SetActive(false);
    }

    public void ShowNextButton()
    {
        nextButton.gameObject.SetActive(true);
    }

    public void NextButton()
    {
        NextButtonPressed?.Invoke();
    }
}
