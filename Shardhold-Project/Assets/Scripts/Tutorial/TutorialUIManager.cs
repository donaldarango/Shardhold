using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{

    private static TutorialUIManager _instance;
    public static TutorialUIManager Instance { get { return _instance; } }

    public delegate void NextButtonPressedHandler();
    public static event NextButtonPressedHandler NextButtonPressed;

    public RectTransform tutorialUITransfom;
    public GameObject tutorialUI;
    public GameObject finishTutorialUI;
    public TMP_Text titleText;
    public TMP_Text explanationText;
    public Button nextButton;

    public Vector2 defaultPosition;
    public Vector2 cardPosition;

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
        tutorialUI.SetActive(enabled);
    }

    public void ToggleFinishTutorialUI(bool enabled)
    {
        finishTutorialUI.SetActive(enabled);
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
        Debug.Log("Tutorial Next Button Pressed");
        NextButtonPressed?.Invoke();
    }

    public void SetPosition(TutorialUIPosition tutorialUIPosition)
    {
        switch(tutorialUIPosition)
        {
            case TutorialUIPosition.Default:
                tutorialUITransfom.anchoredPosition = new Vector3(defaultPosition.x, defaultPosition.y, 0);
                break;
            case TutorialUIPosition.Card:
                tutorialUITransfom.anchoredPosition = new Vector3(cardPosition.x, cardPosition.y, 0);
                break;
            default:
                Debug.Log("Tutorial UI Position not found");
                break;
        }
    }
}
