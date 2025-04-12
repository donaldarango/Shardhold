using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField]
    private bool selectLevel = false;
    public GameObject menu;
    public GameObject levelSelector;
    public Button continueButton;
    public GameObject debugLevels;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(continueButton != null)
        {
            continueButton.interactable = SaveLoad.saveLoad.CheckIfDefaultSaveExists();
        }
        if (debugLevels != null)
        {
            debugLevels.SetActive(GameManager.Instance.showDebugLevelsInMenu);
        }
        if (selectLevel == false)
        {
            ShowMainMenuOptions();
        }
        else {
            ShowLevelSelect();
        }
    }
    public void ShowMainMenuOptions()
    {
        selectLevel = false;
        menu.SetActive(true);
        levelSelector.SetActive(false);
    }
    public void ShowLevelSelect() 
    {
        selectLevel = true;
        menu.SetActive(false);
        levelSelector.SetActive(true);
    }

    //TODO: Continue button? Would need to have saves implmemented to put the
    //game state of the furthest progression loaded in 
}
