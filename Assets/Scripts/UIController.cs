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
    [SerializeField] TextMeshProUGUI subscriberGainedText;
    [SerializeField] GameObject summary;
    public Animator anim;

    GameObject results;
    int distance;
    public int subscriber;
    int lastReportedSubscriberCount;
    int sessionSubscribers;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        distanceText = GameObject.Find("DistanceText").GetComponent<Text>();

        results = GameObject.Find("Results");
        results.SetActive(false);
        summary.SetActive(false);

        // Reset session subscribers at the start
        sessionSubscribers = 0;
        UpdateHighScoreText();
        LoadSubscriber();
    }

    void Update()
    {
        distance = Mathf.FloorToInt(player.distance);
        distanceText.text = distance + " Views";

        if (player.isDead)
        {
            Summary();
        }
    }

    void FixedUpdate()
    {
        CheckHighScore();
        UpdateSubscriber();
    }

    void UpdateSubscriber()
    {
        int newSubscribersGained = distance / 100;
        int newSessionSubscribers = newSubscribersGained - lastReportedSubscriberCount;

        if (newSessionSubscribers > 0)
        {
            sessionSubscribers += newSessionSubscribers; // Accumulate in the session
            subscriber += newSessionSubscribers;         // Update total subscriber count
            PlayerPrefs.SetInt("Subscribers", subscriber);
            PlayerPrefs.Save();

            lastReportedSubscriberCount = newSubscribersGained;

            UpdateSubscriberText();

            // Update only the session gain UI
            subscriberGainedText.text = $"+ {sessionSubscribers}";
        }
    }

    void UpdateSubscriberText()
    {
        subscriberText.text = $"Subscribers: {subscriber}";
    }

    public void GainSubscribers(int amount)
    {
        subscriber += amount;
        sessionSubscribers += amount;  // Add to session total as well
        PlayerPrefs.SetInt("Subscribers", subscriber);
        PlayerPrefs.Save();

        UpdateSubscriberText();

        // Show the current session gain
        subscriberGainedText.text = $"+ {sessionSubscribers}";
    }

    void LoadSubscriber()
    {
        subscriber = PlayerPrefs.GetInt("Subscribers", 0);
        UpdateSubscriberText();
    }

    void CheckHighScore()
    {
        if (distance > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", distance);
            PlayerPrefs.Save();
        }
    }

    void UpdateHighScoreText()
    {
        highScoreText.text = $"HighScore: {PlayerPrefs.GetInt("HighScore", 0)} Views";
    }

    void Summary()
    {
        summary.SetActive(true);
        anim.Play("SummaryScreen");
        subscriberGainedText.text = $"+ {sessionSubscribers}";
    }
}