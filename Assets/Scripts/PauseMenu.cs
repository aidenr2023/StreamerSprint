using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; 

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;
    public GameObject optionsMenu;
    private PlayerMovement playerMovement; 

    void Start()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                if (!optionsMenu.activeSelf)
                {
                    ResumeGame();
                }
                else
                {
                    CloseOptionsMenu();
                }
            }
        }
    }

    public void PauseButton()
    {
        if (!isPaused)
        {
            PauseGame();
        }
        else if (!optionsMenu.activeSelf)
        {
            ResumeGame();
        }

        
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        DisablePlayerControls(); 
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        optionsMenu.SetActive(false);
        EnablePlayerControls(); 
    }

    public void OpenOptionsMenu()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        DisablePlayerControls(); 
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        EnablePlayerControls(); 
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

    
    private void DisablePlayerControls()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false; 
        }
    }

    private void EnablePlayerControls()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true; 
        }
    }
}