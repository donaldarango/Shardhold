using UnityEngine;

public class IndicatorUIManager : MonoBehaviour
{
    public static IndicatorUIManager Instance { get; private set; }

    public Canvas damageCanvas;

    void Awake()
    {
        // Optional: Replace with local-scene-safe Singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple IndicatorUIManagers detected, destroying extra.");
            Destroy(this);
        }
    }
}
