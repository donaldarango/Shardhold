using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadSceneIndex : MonoBehaviour
{   
    public void LoadByIndex(int index) 
    {
        SceneManager.LoadScene(index);
    }
}
