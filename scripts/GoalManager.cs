// gps and mission logic: stores landmarks and handles goal selection

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GoalManager : MonoBehaviour
{
    [System.Serializable]
    public struct Landmark {
        public string name;        // e.g. "kitchen"
        public Transform location; // spot in 3d space
    }

    // connections
    public List<Landmark> landmarks = new List<Landmark>();
    public DroneCommentator commentator;

    private int preExistingCount = 0;

    void Start(){
        // auto-discover landmarks on launch
        FindAllLandmarks();
        preExistingCount = landmarks.Count;
    }

    public void SetActiveGoal(string goalName){
        // find spot by name
        Landmark target = landmarks.FirstOrDefault(l => l.name.ToLower() == goalName.ToLower());

        if (target.location != null){
            // update commentator target
            if (commentator != null){
                commentator.currentGoal = target.location;
                commentator.goalReached = false;
            }
            Debug.Log($"goal set to: {target.name}");
        }
        else{
            Debug.LogWarning($"landmark not found: {goalName}");
        }
    }

    public void SetRandomGoal(){
        if (landmarks.Count == 0) return;

        // pick random spot from list
        int rnd = Random.Range(0, landmarks.Count);
        Landmark pick = landmarks[rnd];

        if (commentator != null){
            commentator.currentGoal = pick.location;
            commentator.goalReached = false;
        }

        Debug.Log($"random goal: {pick.name}");
    }

    // scan children of this gameobject for landmarks
    public void FindAllLandmarks(){
        landmarks.Clear();

        foreach (Transform child in transform){
            Landmark l;
            l.name = child.name;
            l.location = child;
            landmarks.Add(l);
        }

        Debug.Log($"found {landmarks.Count} landmarks");
    }

    // add a landmark at runtime (used by goal generator pipeline)
    public void AddLandmark(string name, Transform location){
        Landmark l;
        l.name = name;
        l.location = location;
        landmarks.Add(l);
    }

    // remove landmarks added after initial discovery (AI-generated ones)
    public void ClearGeneratedLandmarks(){
        if (landmarks.Count > preExistingCount){
            landmarks.RemoveRange(preExistingCount, landmarks.Count - preExistingCount);
            Debug.Log($"GoalManager: cleared generated landmarks, {landmarks.Count} remaining");
        }
    }

    // check if a landmark exists
    public bool HasLandmark(string name){
        return landmarks.Any(l => l.name.ToLower() == name.ToLower());
    }

    // get all landmark names (for ui or debug)
    public List<string> GetLandmarkNames(){
        return landmarks.Select(l => l.name).ToList();
    }
}
