using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryMenu : MonoBehaviour
{
  public void Restart()
  {
    SceneManager.LoadScene("EndlessPath");
  }

  public void MainMenu()
  {
    SceneManager.LoadScene("Main Menu");
  }
}
