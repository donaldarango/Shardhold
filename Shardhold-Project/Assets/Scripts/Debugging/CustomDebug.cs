using UnityEngine;

public class CustomDebug : MonoBehaviour
{
    public static CustomDebug instance;
    public bool debugging = true;

    #region Assertion-based Testing
    public bool runAssertionTesting = false;
    #endregion

    //FOR INTIAL SETTING ONLY; USE GET METHODS
    //per-class debugging bool controls (the respective class will check this, rather than holding its own debugging variable):
    public bool deckDebugging = false;
    public bool saveLoadDebugging = false;
    public bool cusmtomMathDebugging = false;

    public static void RanUnimplementedCode(string descriptor = "<no descriptor>")
    {
        if (Debugging())
        {
            Debug.Log("WARNING: A section of code has just been executed that was not completely impelement: " + descriptor + "\nIf this section was completed, please locate the section and remove the RanUnimplementedCode() function call.");
        }
    }

    #region Logic for Determining Whether to Have Debugging Active

    private static bool Debugging()
    {
        return instance != null && instance.debugging;
    }

    public static bool DeckDebugging()
    {
        return Debugging() && instance.deckDebugging;
    }

    public static bool SaveLoadDebugging()
    {
        return Debugging() && instance.saveLoadDebugging;
    }

    public static bool CustomMathDebugging()
    {
        return Debugging() && instance.cusmtomMathDebugging;
    }

    #endregion
}
