using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI subscriberGainedText;
    [SerializeField] GameObject win;

    private PlayerMovement1 player;
    private float timeRemaining = 30f;
    private bool isCountingDown = true;
    private int sessionSubscribers = 0;

    void Start()
    {
        if (countdownText == null)
        {
            Debug.LogError("Countdown Text is not assigned!");
        }

        // Initialize the player reference by finding the player object
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement1>();

        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }
    }

    void Update()
    {
        if (isCountingDown)
        {
            // Decrease the remaining time
            timeRemaining -= Time.deltaTime;

            // If time is up, stop the countdown
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isCountingDown = false;

                Debug.Log("Countdown Complete!");
                Time.timeScale = 0f;  


                Win();
            }

            // Update countdown text with seconds and milliseconds
            countdownText.text = string.Format("{0:0}:{1:00}", Mathf.Floor(timeRemaining), Mathf.Floor((timeRemaining * 100) % 100));
        }
    }

    // Method to start the countdown
    public void StartCountdown()
    {
        timeRemaining = 30f;  // Reset to 30 seconds
        isCountingDown = true;  // Start the countdown
    }

    // Method to stop the countdown (e.g., on user interaction)
    public void StopCountdown()
    {
        isCountingDown = false;
    }

    // Method to handle the summary screen when countdown is finished
    void Win()
    {
        win.SetActive(true); // Activate the summary screen

        if (subscriberGainedText != null)
        {
            subscriberGainedText.text = $"+ {sessionSubscribers}"; // Display the gained subscribers if applicable
        }

        if (player != null)
        {
            player.SetControlsEnabled(false); // Disable player controls
        }
    }

    // Method to close the summary screen (e.g., when the player presses a button to close)
    
}
