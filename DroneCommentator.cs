using UnityEngine;
using TMPro;

public class DroneCommentator : MonoBehaviour
{
    [Header("Connections")]
    [TextArea(3, 10)] // Makes a box in unity Inspector
    public string personaPrompt = "You are a sarcastic robot. Mock the pilot's failures.";
    public AIService aiService;       // Drag 'AI_Manager' here
    public TextMeshProUGUI uiText;    // Drag 'AI_Text' here

    private bool isTalking = false;

    void OnCollisionEnter(Collision collision)
    {
        // Only talk if we hit something hard and aren't already talking
        if (collision.relativeVelocity.magnitude > 2f && !isTalking)
        {
            TriggerCommentary("crash", collision.gameObject.name);
        }
    }

    public void TriggerCommentary(string eventType, string objectHit)
    {
        isTalking = true;
        uiText.text = "AI Thinking..."; // feedback while waiting 

        // prompt 
        string systemPrompt = "You are a Gen Z flight commentator. Use slang like 'cooked', 'bet', 'no cap', and 'skill issue'. Keep it short."; 
        string userPrompt = $"I just caused a {eventType} involving {objectHit}. React.";

        // Call the Brain
        aiService.SendPrompt(systemPrompt, userPrompt, (response) => 
        {
            // This runs when the API replies
            uiText.text = response;
            isTalking = false;
            
            // Clear text after 5 seconds
            CancelInvoke("ClearText");
            Invoke("ClearText", 5f);
        });
    }

    void ClearText() => uiText.text = "";
}