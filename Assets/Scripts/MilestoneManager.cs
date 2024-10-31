using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilestoneManager : MonoBehaviour
{
    [SerializeField] private int[] subscriberMilestones;
    private int currentMilestoneIndex = 0;

    void UpdateSubscriber(int subscriberCount)
    {
        // Check if the player has reached the next milestone
        if (currentMilestoneIndex < subscriberMilestones.Length &&
            subscriberCount >= subscriberMilestones[currentMilestoneIndex])
        {
            UnlockMilestone(currentMilestoneIndex);
            currentMilestoneIndex++; // Move to the next milestone
        }
    }

    private void UnlockMilestone(int milestoneIndex)
    {
        Debug.Log($"Milestone reached! Unlocked item for reaching {subscriberMilestones[milestoneIndex]} subscribers!");
    }
}
