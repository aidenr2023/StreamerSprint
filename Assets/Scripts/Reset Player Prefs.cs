using UnityEngine;

public class ResetPlayerPrefs : MonoBehaviour
{
    // Method to reset all PlayerPrefs data
    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll(); // Deletes all saved PlayerPrefs data
        PlayerPrefs.Save(); // Save changes to ensure PlayerPrefs are cleared
        Debug.Log("PlayerPrefs have been reset.");
    }
}
