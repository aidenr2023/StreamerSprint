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
    [SerializeField] TextMeshProUGUI milestoneProgressText;
    [SerializeField] Slider milestoneSlider;
    [SerializeField] GameObject summary;
    public Animator anim;

    GameObject results;
    int distance;
    public int subscriber;
    int lastReportedSubscriberCount;
    int sessionSubscribers;

    // Milestone fields
    [SerializeField] private int[] milestoneThresholds = { 100, 250, 500, 1000 }; // Milestone subscriber thresholds
    private int nextMilestoneIndex = 0;  // Tracks next milestone

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
            sessionSubscribers += newSessionSubscribers;
            subscriber += newSessionSubscribers;
            PlayerPrefs.SetInt("Subscribers", subscriber);
            PlayerPrefs.Save();

            lastReportedSubscriberCount = newSubscribersGained;

            UpdateSubscriberText();
            subscriberGainedText.text = $"+ {sessionSubscribers}";

            CheckMilestones(); // Check for milestone progression
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
            // Trigger milestone unlock (you could also add animation or sound here)
            Debug.Log($"Milestone reached: {milestoneThresholds[nextMilestoneIndex]} subscribers!");

            nextMilestoneIndex++; // Move to the next milestone
        }
        UpdateMilestoneProgress();
    }

    void UpdateMilestoneProgress()
    {
        if (nextMilestoneIndex < milestoneThresholds.Length)
        {
            int nextMilestone = milestoneThresholds[nextMilestoneIndex];

            // Ensure progressPercentage does not exceed 1
            float progressPercentage = Mathf.Clamp01((float)subscriber / nextMilestone);

            milestoneProgressText.text = $"Next Milestone: {subscriber}/{nextMilestone} ({progressPercentage * 100:F1}%)";

            // Update slider value
            milestoneSlider.value = progressPercentage;
            milestoneSlider.maxValue = 1f; // Slider max value should be 1 for percentage representation
        }
        else
        {
            milestoneProgressText.text = "All milestones reached!";
            milestoneSlider.value = 1f; // Set slider to full if all milestones are reached
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