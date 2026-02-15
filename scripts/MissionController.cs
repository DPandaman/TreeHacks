// master mission pipeline: vision → goals → path → feedback

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MissionController : MonoBehaviour
{
    [Header("Subsystem References")]
    public VisionBridge vision;
    public RescueGoalGenerator generator;
    public PathArchitect architect;
    public DroneCommentator commentator;
    public GoalManager goalManager;
    public Transform droneTransform;

    [Header("UI References")]
    public GameObject promptPanel;
    public TMP_InputField promptInputField;
    public TMP_Dropdown mapDropdown;
    public TMP_Dropdown vibeDropdown;
    public Button startButton;
    public TextMeshProUGUI statusText;

    [Header("Mission Config")]
    [TextArea]
    public string missionPrompt = "Scan scene. Identify structural hazards and potential survivor locations (tables, corners). Prioritize tight gaps.";

    [Header("Init")]
    public float initDelay = 2.0f;

    // state
    private bool missionRunning = false;

    void Start()
    {
        Invoke("ShowPromptPanel", initDelay);

        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
    }

    public void ShowPromptPanel()
    {
        if (promptPanel == null) return;
        promptPanel.SetActive(true);

        // populate map dropdown
        if (mapDropdown != null)
        {
            mapDropdown.ClearOptions();
            List<string> mapOptions = new List<string>();
            if (commentator != null)
            {
                string[] names = commentator.GetMapNames();
                mapOptions.AddRange(names);
            }
            if (mapOptions.Count == 0)
                mapOptions.AddRange(new[] { "disaster", "stanford_quad", "freestyle" });
            mapDropdown.AddOptions(mapOptions);
        }

        // populate vibe dropdown
        if (vibeDropdown != null)
        {
            vibeDropdown.ClearOptions();
            vibeDropdown.AddOptions(new List<string> { "default", "serious", "chill", "hype", "drill sergeant" });
        }
    }

    public void HidePromptPanel()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        // read mission prompt
        string userText = promptInputField != null ? promptInputField.text : "";
        if (string.IsNullOrEmpty(userText))
        {
            UpdateStatus("please enter a mission prompt");
            return;
        }

        // read map selection
        if (mapDropdown != null && commentator != null)
        {
            string mapName = mapDropdown.options[mapDropdown.value].text;
            commentator.SetMapProfile(mapName);
        }

        // read vibe selection
        if (vibeDropdown != null && commentator != null)
        {
            string vibe = vibeDropdown.options[vibeDropdown.value].text;
            commentator.SetVibe(vibe);
        }

        HidePromptPanel();
        StartMissionFromPrompt(userText);
    }

    // trigger via ui button (original entry point)
    public void StartMissionGeneration()
    {
        if (missionRunning){
            Debug.LogWarning("MissionController: mission already running");
            return;
        }

        // validate required subsystems
        if (vision == null || generator == null || architect == null){
            Debug.LogError("MissionController: missing subsystem references");
            return;
        }

        missionRunning = true;
        Debug.Log("--- MISSION START ---");
        UpdateStatus("scanning environment...");

        // step 1: perception - async vlm scan with mission-aware prompt
        string compositePrompt = BuildVLMPrompt(missionPrompt);
        vision.ScanScene(compositePrompt, OnVisionComplete);
    }

    // trigger via text input field (user types a custom prompt)
    public void StartMissionFromPrompt(string userPrompt){
        if (string.IsNullOrEmpty(userPrompt)){
            Debug.LogWarning("MissionController: empty prompt");
            return;
        }

        missionPrompt = userPrompt;
        StartMissionGeneration();
    }

    string BuildVLMPrompt(string userMission)
    {
        return "Describe what you see in detail. Focus on objects, structures, obstacles, open spaces, spatial layout. " +
               $"The user's mission is: {userMission}. Identify anything relevant to this mission.";
    }

    // callback from vision bridge
    void OnVisionComplete(string aiResponse)
    {
        if (string.IsNullOrEmpty(aiResponse)){
            Debug.LogError("MissionController: vlm returned empty response");
            UpdateStatus("vision scan failed");
            missionRunning = false;
            return;
        }

        UpdateStatus("generating waypoints...");

        // step 2: generate goals from vlm output via GPT-4o
        Transform ref_ = droneTransform != null ? droneTransform : Camera.main.transform;
        generator.GenerateGoalsFromAI(aiResponse, missionPrompt, ref_, OnGoalsGenerated);
    }

    void OnGoalsGenerated(bool success)
    {
        UpdateStatus("computing flight path...");

        // check if user gave directional hints
        bool hasDirection = HasDirectionalHint(missionPrompt);

        // build path through waypoints
        architect.BuildRescuePath(generator.activeGoals, optimizeOrder: !hasDirection);

        // announce results
        int count = generator.activeGoals.Count;
        string stats = $"{count} points of interest";
        string status = success ? "path computed" : "fallback pattern used";

        if (commentator != null)
            commentator.Announce("Mission Plan Generated", $"{stats}. {status}.");

        UpdateStatus($"mission ready: {stats}");
        Debug.Log($"<color=green>MISSION READY:</color> {stats}. pipeline complete.");
        missionRunning = false;
    }

    bool HasDirectionalHint(string prompt)
    {
        if (string.IsNullOrEmpty(prompt)) return false;
        string lower = prompt.ToLower();
        string[] hints = { "north", "south", "east", "west", "start from", "clockwise",
                           "counterclockwise", "left to right", "right to left", "in order" };
        foreach (string hint in hints)
        {
            if (lower.Contains(hint)) return true;
        }
        return false;
    }

    void UpdateStatus(string message)
    {
        Debug.Log($"MissionController: {message}");
        if (statusText != null) statusText.text = message;
    }

    // set a landmark goal by name (from goal manager)
    public void SetGoalByName(string goalName){
        if (goalManager != null)
            goalManager.SetActiveGoal(goalName);
        else
            Debug.LogWarning("MissionController: goal manager not assigned");
    }

    // pick a random landmark goal
    public void SetRandomGoal(){
        if (goalManager != null)
            goalManager.SetRandomGoal();
        else
            Debug.LogWarning("MissionController: goal manager not assigned");
    }

    public void CancelMission(){
        missionRunning = false;
        UpdateStatus("mission cancelled");
        Debug.Log("MissionController: mission cancelled");
    }
}
