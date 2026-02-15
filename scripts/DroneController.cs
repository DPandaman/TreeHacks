// drone controlling logic 
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; 

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class DroneController : MonoBehaviour
{
    [Header("Mode")]
    public bool acroMode = true; // Uncheck this in Inspector if you want Angle mode back

    [Header("Acro Settings (Rate Mode)")]
    public float acroSensitivity = 5f; // How fast it spins (Deg/Sec ish)
    [Range(1f, 3f)] public float stickExpo = 2.0f; // Higher = Softer center stick

    [Header("Angle Settings (Stabilized)")]
    public float angleLimit = 45f; // Max tilt in Angle mode
    public float stabilityPower = 3f;

    [Header("Throttle Power")]
    public float throttlePower = 30f; // Needs to be high for Acro punches

    [Header("Inputs")]
    public InputActionReference throttleAction; 
    public InputActionReference cyclicAction;   
    public InputActionReference yawAction;      

    [Header("UI")]
    public TextMeshProUGUI throttleText; 
    public TextMeshProUGUI altitudeText; 
    public TextMeshProUGUI angleText;    
    public TextMeshProUGUI modeText;     // New UI for "ACRO" vs "ANGLE"

    private Rigidbody rb;
    private AudioSource droneAudio;
    private Vector2 cyclicInput;
    public float throttleInput;
    private float yawInput;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        droneAudio = GetComponent<AudioSource>();
        
        rb.mass = 1.0f; 
        rb.linearDamping = 0.5f; 
        // In Acro, we need high angular drag so the drone stops spinning 
        // when you let go of the stick.
        rb.angularDamping = 8.0f; 
        rb.interpolation = RigidbodyInterpolation.Interpolate; 
    }

    void OnEnable() {
        if(throttleAction) throttleAction.action.Enable();
        if(cyclicAction) cyclicAction.action.Enable();
        if(yawAction) yawAction.action.Enable();
        if(droneAudio) droneAudio.Play();
    }

    void OnDisable() {
        if(throttleAction) throttleAction.action.Disable();
        if(cyclicAction) cyclicAction.action.Disable();
        if(yawAction) yawAction.action.Disable();
    }

    void Update(){
        // 1. Read Inputs
        Vector2 rawCyclic = (cyclicAction) ? cyclicAction.action.ReadValue<Vector2>() : Vector2.zero;
        float rawYaw = (yawAction) ? yawAction.action.ReadValue<float>() : 0f;
        float rawThrottle = (throttleAction) ? throttleAction.action.ReadValue<float>() : -1f;

        // 2. Apply EXPO (Make center stick less sensitive)
        // We use Mathf.Sign to keep negative values negative
        cyclicInput.x = Mathf.Pow(Mathf.Abs(rawCyclic.x), stickExpo) * Mathf.Sign(rawCyclic.x);
        cyclicInput.y = Mathf.Pow(Mathf.Abs(rawCyclic.y), stickExpo) * Mathf.Sign(rawCyclic.y);
        yawInput = Mathf.Pow(Mathf.Abs(rawYaw), stickExpo) * Mathf.Sign(rawYaw);

        throttleInput = (rawThrottle + 1f) / 2f; 

        // 3. Audio & UI
        if (droneAudio) {
            droneAudio.pitch = Mathf.Lerp(1f, 2.5f, throttleInput);
            droneAudio.volume = Mathf.Lerp(0.2f, 1f, throttleInput);
        }
        UpdateUI();
    }

    void FixedUpdate(){
        // 1. Throttle (Always Up relative to drone)
        rb.AddForce(transform.up * (throttleInput * throttlePower)); 

        if (acroMode) {
            HandleAcroMode();
        } else {
            HandleAngleMode();
        }
    }

    void HandleAcroMode() {
        // In Acro, Stick = Rotational Force (Torque)
        // Because we have high Angular Drag (8.0), this acts like a "Rate Controller"
        // Stick Push -> Torque -> Speed limit reached against Drag -> Constant Rotation
        
        Vector3 pitchTorque = Vector3.right * cyclicInput.y * acroSensitivity;
        Vector3 rollTorque  = Vector3.back * cyclicInput.x * acroSensitivity;
        Vector3 yawTorque   = Vector3.up * yawInput * acroSensitivity;

        rb.AddRelativeTorque(pitchTorque + rollTorque + yawTorque);
    }

    void HandleAngleMode() {
        // 1. Clamp the input so we can't request more than 45 degrees
        // (This is a simplified stabilization for backup)
        
        // Calculate "Upright" correction
        Vector3 predictedUp = Quaternion.AngleAxis(
            rb.angularVelocity.magnitude * Mathf.Rad2Deg * stabilityPower / 50f,
            rb.angularVelocity
        ) * transform.up;

        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        rb.AddTorque(torqueVector * stabilityPower * stabilityPower);
        
        // Allow very slow rotation input on top
        rb.AddRelativeTorque(Vector3.right * cyclicInput.y * 2f); 
        rb.AddRelativeTorque(Vector3.back * cyclicInput.x * 2f);
        rb.AddRelativeTorque(Vector3.up * yawInput * 2f);
    }

    void UpdateUI() {
        if (throttleText) throttleText.text = $"THRL {Mathf.Round(throttleInput * 100)}%";
        if (altitudeText) altitudeText.text = $"{Mathf.Round(transform.position.y)}m";
        if (angleText) angleText.text = $"{Mathf.Round(Vector3.Angle(Vector3.up, transform.up))}°";
        if (modeText) modeText.text = acroMode ? "MODE: ACRO" : "MODE: ANGLE";
    }
}