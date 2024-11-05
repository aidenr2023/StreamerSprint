using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
    }

    void Update()
    {
        distance = Mathf.FloorToInt(player.distance);
        distanceText.text = distance + " Views";

        if (player.isDead)
        {
            Summary();
        }

        if(subscriber >= 25)
        {
            canSpawnSewer = true;
        }

        if(subscriber == 50)
        {
            subscriber += 10;
        }
        if (subscriber >= 150)
        {
            canSpawnPigeon = true;
        }

        if(subscriber >= 300)
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
        }
        else
        {
            milestoneProgressText.text = "All milestones reached!";
            milestoneSlider.value = 1f;
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