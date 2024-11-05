using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMilestone", menuName = "Milestone System/Milestone")]
public class Milestone : ScriptableObject
{
        public int targetSubscribers;
        public string rewardDescription;
        public int bonusSubscribers; // Example reward
        public bool unlockNewMechanic; // Flag for new mechanic unlock
                                       // Add other rewards as needed (e.g., item, ability, etc.
}
