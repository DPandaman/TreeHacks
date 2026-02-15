// class for real-time generative commentary

using UnityEngine;
using TMPro;

public class DroneCommentator : MonoBehaviour
{
    // connections
    public AIService aiService;
    public TextMeshProUGUI uiText;

    // settings
    [TextArea(3, 10)]
    public string personaPrompt = "You are a Gen Z flight commentator. Use slang like 'cooked', 'bet', 'no cap', and 'skill issue'. Keep it short.";
    
    // state variables 
    private bool isTalking = false;
    private float idleTimer = 0f;
    private Rigidbody rb;

    void Start()
    {
        // get physics ref
        rb = GetComponent<Rigidbody>(); 
    }

    void Update()
    {
        // check speeding
        float speed = rb.linearVelocity.magnitude; //get speed 
        if (speed > 15f && !isTalking){
            TriggerCommentary("speeding", "nothing");
        }

        // check if flying unstable 
        if (rb.angularVelocity.magnitude > 5f && !isTalking){
             TriggerCommentary("losing control", "gravity");
        }

        // check idle
        if (speed < 0.1f){
            idleTimer += Time.deltaTime;
        }
        else{
            idleTimer = 0f;
        }

        // trigger if idle too long
        if (idleTimer > 10f && !isTalking){
            TriggerCommentary("idle", "nothing");
            idleTimer = 0f;
        }

        // trigger if u almost hit smth but didn't 
        // cast ray forward 1 meter
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1.0f)){
            // check if we are moving fast toward it
            if (rb.linearVelocity.magnitude > 5f && !isTalking){
                TriggerCommentary("near miss", hit.collider.name);
            }
        }

        // check if stuck 
        float throttleInput = droneCtrl.throttleValue;  // check high throttle 
        // full throttle but not moving
        if (Mathf.Abs(throttleInput) > 0.8f && rb.linearVelocity.magnitude < 0.1f && !isTalking){
            TriggerCommentary("stuck", "wall");
        }

        // check if updside down 
        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f && !isTalking){// check dot product of up vector
            TriggerCommentary("upside down", "gravity");
        }
    }

    void OnCollisionEnter(Collision collision){
        // runs only if we crash into smth hard 
        if (collision.relativeVelocity.magnitude > 2f && !isTalking){
            TriggerCommentary("crash", collision.gameObject.name); //tells u what object u hit 
        }
    }

    public void TriggerCommentary(string eventType, string objectHit){
        // set talking state
        isTalking = true;
        uiText.text = "AI Thinking...";

        // calc mph
        float speedMph = rb.linearVelocity.magnitude * 2.237f;

        // build prompts
        string systemPrompt = personaPrompt;
        string userPrompt = $"I just caused a {eventType} event involving {objectHit} at {speedMph:F1} MPH. React.";

        // call ai
        aiService.SendPrompt(systemPrompt, userPrompt, (response) => 
        {
            uiText.text = response;
            
            // cooldown 5s
            Invoke("ResetTalking", 5f);
        });
    }

    void ResetTalking() => isTalking = false;
}