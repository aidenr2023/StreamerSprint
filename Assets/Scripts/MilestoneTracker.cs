using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MilestoneTracker : MonoBehaviour
{
    public Slider milestoneSlider;
    public TextMeshProUGUI milestoneText;
    public int milestoneTarget = 1000; // Set your milestone target

    private int subscriberCount;

    void Start()
    {
        // Initialize the milestone slider
        milestoneSlider.maxValue = milestoneTarget;
        UpdateMilestoneUI();
    }

    public void UpdateSubscriberCount(int newCount)
    {
        subscriberCount = newCount;
        UpdateMilestoneUI();

        // Check if milestone is reached
        if (subscriberCount >= milestoneTarget)
        {
            // Unlock milestone reward or notify player
            Debug.Log("Milestone reached! Reward unlocked.");
        }
    }

    private void UpdateMilestoneUI()
    {
        milestoneSlider.value = subscriberCount;
        milestoneText.text = $"{subscriberCount} / {milestoneTarget} Subscribers";
    }
}
