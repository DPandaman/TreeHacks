using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Needed for fancy list searching

public class GoalManager : MonoBehaviour
{
    [System.Serializable]
    public struct Landmark {
        public string name;      // e.g. "Kitchen"
        public Transform location; // The empty GameObject at that spot
    }

    public List<Landmark> landmarks; // Drag all your room spots here
    public DroneCommentator commentator; // Drag your drone here

    // Call this from your UI or AI script
    public void SetActiveGoal(string goalName)
    {
        // Find the landmark with the matching name (case-insensitive)
        Landmark target = landmarks.FirstOrDefault(l => l.name.ToLower() == goalName.ToLower());

        if (target.location != null)
        {
            // Tell the commentator: "This is your new target"
            commentator.currentGoal = target.location;
            Debug.Log($"Goal set to: {target.name}");
        }
        else
        {
            Debug.LogWarning($"Could not find landmark: {goalName}");
        }
    }

    // Optional: Cycle to the next random goal for a game mode
    public void SetRandomGoal()
    {
        if (landmarks.Count > 0)
        {
            int rnd = Random.Range(0, landmarks.Count);
            commentator.currentGoal = landmarks[rnd].location;
        }
    }
}