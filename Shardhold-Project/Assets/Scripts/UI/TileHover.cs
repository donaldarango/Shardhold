using UnityEngine;

public class TileHover : MonoBehaviour
{
    private void OnEnable()
    {
        MapGenerator.HoverTile += OnHover;
    }

    private void OnDisable()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnHover (TileActor ta)
    {
        if (ta != null)
        {
            PrintStats(ta);
        }
    }

    void PrintStats(TileActor ta)
    {
        ta.ShowStats();
    }
}
