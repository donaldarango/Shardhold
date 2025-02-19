using UnityEngine;

public class CustomDebug : MonoBehaviour
{
    public static CustomDebug instance;
    public bool debugging = true;

    //per-class debugging bool controls:
    public bool deckDebugging = true;
    public bool SaveLoadDebugging = true;

    public static void RanUnimplementedCode(string descriptor = "<no descriptor>")
    {
        if (Debugging())
        {
            Debug.Log("WARNING: A section of code has just been executed that was not completely impelement: " + descriptor + "\nIf this section was completed, please locate the section and remove the RanUnimplementedCode() function call.");
        }
    }


    private static bool Debugging()
    {
        return instance != null && instance.debugging;
    }
}
