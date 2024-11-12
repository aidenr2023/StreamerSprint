using UnityEngine;
using TMPro;

public class BirdCount : MonoBehaviour
{
    public int birdCount; // Track the bird count
    public TextMeshProUGUI birdText; // Display the bird count
    public GameObject win; // Win screen UI element
    [SerializeField] TextMeshProUGUI subscriberGainedText; // Show subscriber count on win screen
    public TextMeshProUGUI subscriberText; // New field for subscriber text
    public UIController1 uiController; // Reference to UIController1 to access subscriber count

    void Start()
    {
        win.SetActive(false); // Initially hide the win screen
    }

    void Update()
    {
        birdText.text = birdCount.ToString(); // Update bird count UI
        CheckWinCondition(); // Check if the win condition is met
    }

    void CheckWinCondition()
    {
        if (birdCount >= 5) // Trigger win condition when bird count reaches 10
        {
            win.SetActive(true); // Show win screen
            Time.timeScale = 0; // Pause the game
           
            // Display the session subscribers on the win screen
            subscriberGainedText.text = $"+ {uiController.sessionSubscribers}";
            subscriberText.text = $"Subscribers: {uiController.subscriber}"; // Show the actual subscriber count
        }
    }
}
