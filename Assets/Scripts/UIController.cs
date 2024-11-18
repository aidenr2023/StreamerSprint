using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    PlayerMovement player; // Reference to PlayerMovement
    Text distanceText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI subscriberText;
    [SerializeField] TextMeshProUGUI subscriberGainedText;
    [SerializeField] TextMeshProUGUI milestoneProgressText;
    [SerializeField] private Sprite[] milestoneSprites;
    [SerializeField] private Image milestoneImage;
    [SerializeField] Slider milestoneSlider;
    [SerializeField] GameObject summary; // Summary screen object
    [SerializeField] GameObject milestone1;
    [SerializeField] GameObject milestone2;
    [SerializeField] GameObject milestone3;
    [SerializeField] GameObject milestone4;


    public bool canSpawnPigeon = false;
    public bool canSpawnSewer = false;
    public bool canDoTricks;

    GameObject results;
    int distance;
    public int subscriber;
    int lastReportedSubscriberCount;
    int sessionSubscribers;

    // Milestone fields
    [SerializeField] private int[] milestoneThresholds = { 100, 250, 500, 1000 };
    private int nextMilestoneIndex = 0;
    private HashSet<int> shownMilestones = new HashSet<int>();

    // Rewards
    [SerializeField] private string[] milestoneRewards = { "Milestone 1", "Bonus Subscribers", "Milestone 2", "Milestone 3" };

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        distanceText = GameObject.Find("DistanceText").GetComponent<Text>();

        results = GameObject.Find("Results");
        results.SetActive(false);
        summary.SetActive(false);

        sessionSubscribers = 0;
        UpdateHighScoreText();
        LoadSubscriber();
        UpdateMilestoneProgress();
        UpdateMilestoneImage();
        CheckMilestones();
    }

    void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Challenge1")
        {
        canSpawnSewer = true;
        canSpawnPigeon = true;
        canDoTricks = true; 
        }
        
        distance = Mathf.FloorToInt(player.distance);
        distanceText.text = distance + " Views";

        if (player.isDead)
        {
            Summary();
            Time.timeScale = 0;
        }
        else
        {
            CheckHighScore();
        }

        if (subscriber >= 25)
        {
            canSpawnSewer = true;
        }

        if (subscriber == 50)
        {
            subscriber += 10;
        }
        if (subscriber >= 150)
        {
            canSpawnPigeon = true;
        }

        if (subscriber >= 300)
        {
            canDoTricks = true;
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
            sessionSubscribers += newSessionSubscribers;
            subscriber += newSessionSubscribers;
            PlayerPrefs.SetInt("Subscribers", subscriber);
            PlayerPrefs.Save();

            lastReportedSubscriberCount = newSubscribersGained;

            UpdateSubscriberText();
            subscriberGainedText.text = $"+ {sessionSubscribers}";

            CheckMilestones();
        }
    }

    void UpdateSubscriberText()
    {
        subscriberText.text = $"Subscribers: {subscriber}";
    }

    void CheckMilestones()
    {
        while (nextMilestoneIndex < milestoneThresholds.Length && subscriber >= milestoneThresholds[nextMilestoneIndex])
        {
            Debug.Log($"Milestone reached: {milestoneThresholds[nextMilestoneIndex]} subscribers!");

            nextMilestoneIndex++;
        }
        UpdateMilestoneProgress();
    }

    void UpdateMilestoneProgress()
    {
        if (nextMilestoneIndex < milestoneThresholds.Length)
        {
            int nextMilestone = milestoneThresholds[nextMilestoneIndex];
            float progressPercentage = Mathf.Clamp01((float)subscriber / nextMilestone);

            milestoneProgressText.text = $"Next Milestone: {subscriber}/{nextMilestone} ({progressPercentage * 100:F1}%)";
            milestoneSlider.value = progressPercentage;
            milestoneSlider.maxValue = 1f;
            UpdateMilestoneImage();
        }
        else
        {
            milestoneProgressText.text = "All milestones reached!";
            milestoneSlider.value = 1f;
            milestoneImage.sprite = null;
        }
    }

    void UpdateMilestoneImage()
    {
        // Show the current milestone icon if it exists in the array
        if (nextMilestoneIndex < milestoneSprites.Length && milestoneSprites[nextMilestoneIndex] != null)
        {
            milestoneImage.sprite = milestoneSprites[nextMilestoneIndex];
        }
        else
        {
            milestoneImage.sprite = null; // Hide sprite if no sprite available
        }
    }

    public void GainSubscribers(int amount)
    {
        subscriber += amount;
        sessionSubscribers += amount;
        PlayerPrefs.SetInt("Subscribers", subscriber);
        PlayerPrefs.Save();

        UpdateSubscriberText();
        subscriberGainedText.text = $"+ {sessionSubscribers}";

        CheckMilestones();
    }
    public void LoseSubscribers(int amount)
    {
        subscriber -= amount;
        sessionSubscribers -= amount;
        PlayerPrefs.SetInt("Subscribers", subscriber);
        PlayerPrefs.Save();

        UpdateSubscriberText();
        subscriberGainedText.text = $" {sessionSubscribers}";
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
            UpdateHighScoreText();
        }
    }

    void UpdateHighScoreText()
    {
        highScoreText.text = $"HighScore: {PlayerPrefs.GetInt("HighScore", 0)} Views";
    }

    void Summary()
    {
        summary.SetActive(true);
        if (sessionSubscribers < 0)
        {
            subscriberGainedText.text = $" {sessionSubscribers}";
        }
        else
        {
            subscriberGainedText.text = $" + {sessionSubscribers}";
        }

        // Check each milestone and show it only if it hasn't been shown this session
        if (subscriber > 25 && !shownMilestones.Contains(25))
        {
            milestone1.SetActive(true);
            shownMilestones.Add(25);
            summary.SetActive(false);
        }

        if (subscriber > 50 && !shownMilestones.Contains(50))
        {
            milestone2.SetActive(true);
            shownMilestones.Add(50);
            summary.SetActive(false);
        }

        if (subscriber > 150 && !shownMilestones.Contains(150))
        {
            milestone3.SetActive(true);
            shownMilestones.Add(150);
            summary.SetActive(false);
        }

        if (subscriber > 300 && !shownMilestones.Contains(300))
        {
            milestone4.SetActive(true);
            shownMilestones.Add(300);
            summary.SetActive(false);
        }

        player.SetControlsEnabled(false);
    }

    public void CloseSummary()
    {
        summary.SetActive(false);
        player.SetControlsEnabled(true); 
    }
}