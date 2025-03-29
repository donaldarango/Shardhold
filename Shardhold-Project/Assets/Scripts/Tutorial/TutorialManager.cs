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
        SetNextTutorial(currentTutorial.order + 1);
    }

    public void SetNextTutorial(int currentOrder)
    {
        currentTutorial = GetTutorialByOrder(currentOrder);

        if(!currentTutorial)
        {
            TutorialUIManager.Instance.ToggleTutorialUI(false);

            // Resume and Enable Turn Timer
            UIManager.Instance.ResumeTurnTimer();
            UIManager.Instance.EnableTimerButton();

            // Add tutorial completion text

            return;
        }

        if (currentTutorial.pausesTimer)
        {
            UIManager.Instance.PauseTurnTimer();
        }
        else
        {
            UIManager.Instance.ResumeTurnTimer();
        }

        if (currentTutorial.disablesTimer)
        {
            UIManager.Instance.DisableTimerButton();
        }
        else
        {
            UIManager.Instance.EnableTimerButton();
        }

        // Set explanation text in UI
        TutorialUIManager.Instance.SetTitleText(currentTutorial.title);
        TutorialUIManager.Instance.SetExplanationText(currentTutorial.explanation);
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
}
