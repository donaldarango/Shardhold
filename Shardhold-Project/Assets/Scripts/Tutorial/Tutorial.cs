using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public int order;
    public string title;
    [TextArea(5,10)]
    public string explanation = "";
    public bool pausesTimer = true;
    public bool disablesTimer = true;

    //public virtual void Start()
    //{
    //    if (TutorialManager.Instance == null)
    //    {
    //        Debug.LogError("Tutorial Manager not initialized");
    //        return;
    //    }
    //    TutorialManager.Instance.AddTutorialToList( this );
    //}

    private void Awake()
    {
        TutorialManager.Instance.AddTutorialToList(this);
    }

    public virtual void TutorialStart() { }

    public virtual void CheckIfHappening() { }
}
