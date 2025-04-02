using System;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public enum TutorialSetting
{
    pausesTimer,
    resumesTimer,
    disablesTimer,
    enablesTimer,
    disablesCards,
    enablesCards,
}

[Serializable]
public enum TutorialUIPosition
{
    Default,
    Card,
}

public class Tutorial : MonoBehaviour
{
    public bool completed = false;
    public int order;
    public string title;
    [TextArea(5,10)]
    public string explanation = "";
    public TutorialSetting[] settings;
    public TutorialUIPosition position;

    private void Awake()
    {
        TutorialManager.Instance.AddTutorialToList(this);
    }

    public virtual void TutorialStart() { }

    public virtual void CheckIfHappening() { }
}
