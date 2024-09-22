using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
   public GameObject pauseMenu;
   public static bool isPaused;
   public GameObject optionsMenu;
   
  

    void Start()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused)
            {
                PauseGame();
            }

            else
            {

                if(!optionsMenu.activeSelf)
            {
                ResumeGame();
            }
            }
         
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

    }

 

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        optionsMenu.SetActive(false);

    }

     public void OptionsMenu()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;

    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

