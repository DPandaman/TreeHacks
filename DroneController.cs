sing UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Drone Settings")]
    public float throttlePower = 20f;  // Vertical lift
    public float cyclicPower = 5f;     // Pitch & Roll speed (Tilt)
    public float yawPower = 2f;        // Spinning speed

    [Header("Input References")]
    public InputActionReference throttleAction; // Left Stick Y
    public InputActionReference cyclicAction;   // Right Stick (X/Y)
    public InputActionReference yawAction;      // Left Stick X

    private Rigidbody rb;
    private Vector2 cyclicInput;
    private float throttleInput;
    private float yawInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Make the drone lighter or heavier here if needed
        rb.mass = 1.0f; 
        rb.drag = 1.0f; // Air resistance helps stabilize it
        rb.angularDrag = 2.0f; // Stops it from spinning forever
    }

    void Update()
    {
        // 1. Read Input Values
        // (We read in Update, but apply physics in FixedUpdate)
        if (cyclicAction != null) cyclicInput = cyclicAction.action.ReadValue<Vector2>();
        if (yawAction != null) yawInput = yawAction.action.ReadValue<float>();
        
        // 2. Handle Throttle Remapping
        // Gamepads give -1 to 1. We need 0 to 1 for throttle.
        float rawThrottle = (throttleAction != null) ? throttleAction.action.ReadValue<float>() : -1f;
        throttleInput = (rawThrottle + 1f) / 2f; 
    }

    void FixedUpdate()
    {
        HandlePropellers();
        HandleStabilization();
    }

    void HandlePropellers()
    {
        // 1. Apply Upward Force (Throttle)
        // Physics: F = ma. We add force relative to the drone's "Up" direction.
        Vector3 liftForce = Vector3.up * (throttleInput * throttlePower);
        rb.AddRelativeForce(liftForce);

        // 2. Apply Rotation (Pitch, Roll, Yaw)
        // Pitch (Forward/Back tilting)
        rb.AddRelativeTorque(Vector3.right * cyclicInput.y * cyclicPower);
        // Roll (Left/Right tilting)
        rb.AddRelativeTorque(Vector3.back * cyclicInput.x * cyclicPower);
        // Yaw (Turning left/right)
        rb.AddRelativeTorque(Vector3.up * yawInput * yawPower);
    }
    
    // Simple "Hover" helper to keep it upright when you let go
    void HandleStabilization()
    {
        // Real FPV drones don't self-level (Acro mode), 
        // but for a hackathon demo, a little stability helps prevents crashes.
        if (cyclicInput.magnitude < 0.1f)
        {
            // Slowly rotate back to flat (Vector3.up)
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 2.0f);
        }
    }
}