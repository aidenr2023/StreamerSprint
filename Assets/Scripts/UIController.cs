using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    PlayerMovement player;
    Text distanceText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI subscriberText;

    GameObject results;
    int distance;
    int subscriber;
    int lastReportedSubscriberCount;

    // Start is called before the first frame update
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        distanceText = GameObject.Find("DistanceText").GetComponent<Text>();

        results = GameObject.Find("Results");
        results.SetActive(false);
        UpdateHighScoreText();
        LoadSubscriber(); // Load subscriber count from PlayerPrefs
    }

    // Update is called once per frame
    void Update()
    {
        distance = Mathf.FloorToInt(player.distance);
        distanceText.text = distance + "m";

        if (player.isDead)
        {
            results.SetActive(true);
            Invoke("RestartScene", 1f); // For the purpose of the prototype, remove later
        }
    }
    
    void FixedUpdate()
    {
        // Call both high score and subscriber updates
        CheckHighScore();
        UpdateSubscriber();
    }

    void UpdateSubscriber()
    {
    // Calculate the new subscribers gained in the current session
    int newSubscribersGained = distance / 100;

    // Calculate the actual subscribers this session should add, relative to the previous session
    int sessionSubscribers = newSubscribersGained - lastReportedSubscriberCount;

    // Update the subscriber count only if there's new subscribers to add
    if (sessionSubscribers > 0)
    {
        // Add the session's new subscribers to the current total
        subscriber += sessionSubscribers;

        // Save the current subscriber count to PlayerPrefs
        PlayerPrefs.SetInt("Subscribers", subscriber);
        PlayerPrefs.Save(); // Save immediately

        // Update last reported subscriber count to prevent adding the same amount again
        lastReportedSubscriberCount = newSubscribersGained;

        // Update the UI
        UpdateSubscriberText();

        // Log the update for debugging
        Debug.Log($"New Subscribers Gained: {sessionSubscribers}, Total Subscribers Updated: {subscriber}");
    }
    else
    {
        Debug.Log("No new subscribers gained this update.");
    }
}

    void UpdateSubscriberText()
    {
        subscriberText.text = $"Subscribers: {subscriber}";
        Debug.Log("Updated Subscribers: " + subscriber); // Debug to check if this is called and has the expected values
    }

    public void GainSubscribers(int amount)
    {
        Debug.Log("GainSubscribers called with amount: " + amount);
        subscriber += amount;
        PlayerPrefs.SetInt("Subscribers", subscriber);
        PlayerPrefs.Save();
        UpdateSubscriberText();
        Debug.Log($"Gained {amount} Subscribers from Skill Move. Total Subscribers: {subscriber}");
    }

    void LoadSubscriber()
    {
        subscriber = PlayerPrefs.GetInt("Subscribers", 0);
        UpdateSubscriberText(); // Update UI with loaded subscribers
        Debug.Log("Loaded Subscribers from PlayerPrefs: " + subscriber);
    }

    void CheckHighScore()
    {
        if (distance > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", distance);
            PlayerPrefs.Save(); // Ensure PlayerPrefs are saved immediately
        }
    }

    void UpdateHighScoreText()
    {
        highScoreText.text = $"HighScore: {PlayerPrefs.GetInt("HighScore", 0)}" + "m";
    }

    void RestartScene()
    {
        SceneManager.LoadScene("EndlessPath");
    }
}
