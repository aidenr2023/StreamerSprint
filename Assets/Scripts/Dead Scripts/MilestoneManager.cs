using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MilestoneManager : MonoBehaviour
{
    [SerializeField] private int[] subscriberMilestones;
    private int currentMilestoneIndex = 0;

    // UnityEvent to notify when each milestone is reached
    public UnityEvent<int> OnMilestoneReached;

    void UpdateSubscriber(int subscriberCount)
    {
        if (currentMilestoneIndex < subscriberMilestones.Length &&
            subscriberCount >= subscriberMilestones[currentMilestoneIndex])
        {
            UnlockMilestone(currentMilestoneIndex);
            OnMilestoneReached?.Invoke(currentMilestoneIndex);  
            currentMilestoneIndex++;
        }
    }

    private void UnlockMilestone(int milestoneIndex)
    {
        Debug.Log($"Milestone reached! Unlocked item for reaching {subscriberMilestones[milestoneIndex]} subscribers!");
    }
}