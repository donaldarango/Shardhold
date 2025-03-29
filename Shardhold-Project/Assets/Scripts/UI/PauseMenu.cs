using UnityEngine;
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    [SerializeField] private bool menuActiveStatus = false;
    [SerializeField] private bool settingsActive = false;
    public static bool isPaused = false;


    void Start()
    {
        if (pauseMenu!= null) 
        {
            pauseMenu.SetActive(false);
            if (Time.timeScale == 0) {
                Time.timeScale = 1;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsActive == false) 
        {
            ToggleMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && settingsActive == true)
        {
            SwapMenu();
        }
        
    }

    public void ToggleMenu() 
    {
        menuActiveStatus = !menuActiveStatus;
        pauseMenu.SetActive(menuActiveStatus);
        if (menuActiveStatus == true) 
        {
            Time.timeScale = 0;
        }
        else {
            Time.timeScale = 1;
        }

        isPaused = !isPaused;
    }

    public void SwapMenu()
    {
        if(menuActiveStatus == true && settingsActive == false)
        {
            pauseMenu.SetActive(!menuActiveStatus);
            settingsMenu.SetActive(menuActiveStatus);
        } else if (menuActiveStatus == true && settingsActive == true)
        {
            pauseMenu.SetActive(menuActiveStatus);
            settingsMenu.SetActive(!menuActiveStatus);
        }

        settingsActive = !settingsActive;
    }
}
