using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;

public class UIController1 : MonoBehaviour
{
    PlayerMovement1 player;
    [SerializeField] TextMeshProUGUI subscriberText;
    [SerializeField] TextMeshProUGUI subscriberGainedText;
    [SerializeField] GameObject lose;
    public Animator anim;

    public bool canSpawnPigeon = false;
    public bool canSpawnSewer = false;
    public bool canDoTricks;

    GameObject results;
    int distance;
    public int subscriber;
    public int lastReportedSubscriberCount;
    public int sessionSubscribers;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement1>();
        lose.SetActive(false);

        sessionSubscribers = 0;
        LoadSubscriber();

        // Check the current scene and unlock features accordingly
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Level 1")  
        {
            UnlockAllFeatures(); // Unlock all features for this specific scene
        }
    }

    void Update()
    {
        if (player.isDead)
        {
            Lose();
        }
    }

    void FixedUpdate()
    {
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
        }
    }

    void UpdateSubscriberText()
    {
        subscriberText.text = $"Subscribers: {subscriber}";
    }

    public void GainSubscribers(int amount)
    {
        subscriber += amount;
        sessionSubscribers += amount;
        PlayerPrefs.SetInt("Subscribers", subscriber);
        PlayerPrefs.Save();

        UpdateSubscriberText();
        subscriberGainedText.text = $"+ {sessionSubscribers}";
    }

    void LoadSubscriber()
    {
        subscriber = PlayerPrefs.GetInt("Subscribers", 0);
        UpdateSubscriberText();
    }

    void Lose()
    {  
        lose.SetActive(true);
        subscriberGainedText.text = $"+ {sessionSubscribers}";
        player.SetControlsEnabled(false);
        Time.timeScale = 0;
    }

    // New method to unlock features in the specified scene
    void UnlockAllFeatures()
    {
        canSpawnSewer = true;
        canSpawnPigeon = true;
        canDoTricks = true;

       
    }
}
