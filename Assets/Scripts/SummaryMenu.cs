using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryMenu : MonoBehaviour
{
    public void Restart()
{
    Time.timeScale = 1;  
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
