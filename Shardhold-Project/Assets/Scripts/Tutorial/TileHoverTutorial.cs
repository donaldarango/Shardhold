using UnityEngine;

public class TileHoverTutorial : Tutorial
{
    
    public int ringNumber;
    public int laneNumber;
    private bool hoveredOver;

    private void OnEnable()
    {
        MapGenerator.HoverTile += OnHoverTile;
    }

    private void OnDisable()
    {
        MapGenerator.HoverTile -= OnHoverTile;
    }

    //public override void Start()
    //{
    //    base.Start();
    //}

    public override void TutorialStart()
    {
        TutorialUIManager.Instance.HideNextButton();
    }

    public override void CheckIfHappening()
    {
        if (hoveredOver == true)
        {
            TutorialManager.Instance.CompletedTutorial();
        }
    }

    public void OnHoverTile(int r, int l)
    {
        int currentOrder = TutorialManager.Instance.GetCurrentOrder();
        if (r == ringNumber && l == laneNumber && currentOrder == order)
        {
            hoveredOver = true;
        }
    }

    
}
