using UnityEngine;

public class CustomDebug : MonoBehaviour
{
    public static CustomDebug instance;

    public enum DebuggingType
    {
        Off = 0,        //print nothing; for final product
        ErrorOnly = 1,  //only print when something is wrong; use when everything should've been taken care of and tested sufficiently
        Warnings = 2,   //only print when something is wrong or could be an issue; default during general development and testing of features
        Normal = 3,     //also print normal informational statements; useful when developing that particular feature
        Verbose = 4     //print as much as possible; ONLY USE WHEN DEBUGGING A PARTICULAR ERROR, PLEASE LEAVE ON SOMETHING ELSE OTHERWISE
    }

    public DebuggingType debugging = DebuggingType.Normal;  //sets the maximum debugging prints amount; even if a particular debugging setting would show more, this stops it

    #region Assertion-based Testing
    public bool runAssertionTesting = false;
    #endregion

    //FOR INTIAL SETTING ONLY; USE GET METHODS
    //per-class debugging bool controls (the respective class will check this, rather than holding its own debugging variable):
    public DebuggingType deckDebugging = DebuggingType.Warnings;
    public DebuggingType saveLoadDebugging = DebuggingType.Warnings;
    public DebuggingType cusmtomMathDebugging = DebuggingType.Warnings;

    public static void RanUnimplementedCode(string descriptor = "<no descriptor>", DebuggingType level=DebuggingType.ErrorOnly)
    {
        if (Debugging(level))
        {
            Debug.Log("WARNING: A section of code has just been executed that was not completely impelemented: " + descriptor + "\nIf this section was completed, please locate the section and remove the RanUnimplementedCode() function call.");
        }
    }

    #region Logic for Determining Whether to Have Debugging Active

    /// <summary>
    /// generally, use a more class-specific debugging check in place of this (those still call this)
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static bool Debugging(DebuggingType level)
    {
        return instance != null && instance.debugging >= level;
    }

    public static bool DeckDebugging(DebuggingType level)
    {
        return Debugging(level) && instance.deckDebugging >= level;
    }

    public static bool SaveLoadDebugging(DebuggingType level)
    {
        return Debugging(level) && instance.saveLoadDebugging >= level;
    }

    public static bool CustomMathDebugging(DebuggingType level)
    {
        return Debugging(level) && instance.cusmtomMathDebugging >= level;
    }

    #endregion
}
