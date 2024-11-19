using System.Collections;  // Required for IEnumerator
using UnityEngine;
using TMPro;

public class IntroText : MonoBehaviour
{
    public TextMeshProUGUI introText;  // Reference to the TextMeshProUGUI element
    public float textDuration = 3f;    // Duration for the text to be displayed

    private void Start()
    {
        // Start the coroutine to display the text
        StartCoroutine(DisplayIntroText());
    }

    private IEnumerator DisplayIntroText()
    {
        // Show the intro text
        introText.gameObject.SetActive(true);

        // Pause the game by setting time scale to 0
      

        // Wait for the specified time duration
        yield return new WaitForSecondsRealtime(textDuration);  // Wait without being affected by time scale

        // Hide the intro text after the duration
        introText.gameObject.SetActive(false);

        // Resume the game by setting time scale back to 1
        Time.timeScale = 1;
    }
}
