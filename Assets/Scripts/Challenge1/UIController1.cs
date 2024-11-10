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
    [SerializeField] GameObject win;
    [SerializeField] GameObject lose;
    public Animator anim;

    public bool canSpawnPigeon = false;
    public bool canSpawnSewer = false;
    public bool canDoTricks;

    GameObject results;
    int distance;
    public int subscriber;
    int lastReportedSubscriberCount;
    int sessionSubscribers;


    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement1>();
        win.SetActive(false);
        lose.SetActive(false);

        sessionSubscribers = 0;
        LoadSubscriber();
    }

    void Update()
    {
        if (player.isDead)
        {
            Lose();  
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
    }

}
