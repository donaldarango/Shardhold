using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;


public class TutorialManager : MonoBehaviour
{

    private static TutorialManager _instance;
    public static TutorialManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("TutorialManager");
                _instance = go.AddComponent<TutorialManager>();
            }
            return _instance;
        }
    }

    [SerializeField] private List<Tutorial> tutorials = new List<Tutorial>();
    [SerializeField] private Tutorial currentTutorial;
    [SerializeField] private int currentTutorialIndex;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetNextTutorial(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTutorial)
        {
            currentTutorial.CheckIfHappening();
        }
    }

    public void CompletedTutorial()
    {
        currentTutorial.StopTileHighlight();
        SetNextTutorial(currentTutorial.order + 1);
    }

    public void SetNextTutorial(int currentOrder)
    {
        currentTutorial = GetTutorialByOrder(currentOrder);
        currentTutorialIndex = currentOrder;

        if (!currentTutorial)
        {
            TutorialUIManager.Instance.ToggleTutorialUI(false);

            // Add tutorial completion text
            TutorialUIManager.Instance.ToggleFinishTutorialUI(true);

            return;
        }

        foreach(TutorialSetting settings in currentTutorial.settings)
        {
            ExecuteTutorialSetting(settings);
        }

        // Setup UI for current tutorial
        TutorialUIManager.Instance.SetTitleText(currentTutorial.title);
        TutorialUIManager.Instance.SetExplanationText(currentTutorial.explanation);
        TutorialUIManager.Instance.SetPosition(currentTutorial.position);
        currentTutorial.TutorialStart();
    }

    public Tutorial GetTutorialByOrder(int order)
    {
        foreach (Tutorial tutorial in tutorials)
        {
            if(tutorial.order == order)
                return tutorial;
        }

        //Debug.Log($"Tutorial with order {order} is not found");
        return null;
    }

    public void AddTutorialToList(Tutorial tutorial)
    {
        tutorials.Add(tutorial);
    }

    public int GetCurrentOrder()
    {
        if(currentTutorial)
            return currentTutorial.order;

        return -1;
    }

    public void ExecuteTutorialSetting(TutorialSetting setting)
    {
        switch(setting)
        {
            case(TutorialSetting.enablesTimer):
                UIManager.Instance.EnableTimerButton();
                break;
            case (TutorialSetting.disablesTimer):
                UIManager.Instance.DisableTimerButton();
                break;
            case (TutorialSetting.pausesTimer):
                UIManager.Instance.PauseTurnTimer();
                break;
            case (TutorialSetting.resumesTimer):
                UIManager.Instance.ResumeTurnTimer();
                break;
            case (TutorialSetting.disablesCards):
                Deck.Instance.TutorialDisableCards();
                break;
            case (TutorialSetting.enablesCards):
                Deck.Instance.TutorialEnableCards();
                break;
        }
    }
}
