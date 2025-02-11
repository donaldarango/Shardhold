using UnityEngine;
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    [SerializeField]
    private bool menuActiveStatus = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            ToggleMenu();
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
    }
}
