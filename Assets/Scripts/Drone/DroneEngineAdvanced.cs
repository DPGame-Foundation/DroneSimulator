using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DroneEngineAdvanced : MonoBehaviour, IEngine
{
    [SerializeField] private Transform propeller;
    [SerializeField] private float propRotSpeed = 300f;

    // New field to control shaking effect
    [SerializeField] private bool enableShake = false; 
    [SerializeField] private float shakeMagnitude = 0.1f; 
    [SerializeField] private float shakeFrequency = 10f;

    // PID controllers for stabilization
    private PIDController altitudePID;
    private PIDController pitchPID;
    private PIDController rollPID;

    // Desired states
    [SerializeField] private float targetAltitude = 5f; // Set your desired altitude
    [SerializeField] private float targetPitch = 0f; // Set your desired pitch
    [SerializeField] private float targetRoll = 0f; // Set your desired roll

    void Start()
    {
        // Initialize PID controllers with suitable gains
        altitudePID = new PIDController(1f, 0.1f, 0f); // Tuning parameters for altitude
        pitchPID = new PIDController(1f, 0.1f, 0f); // Tuning parameters for pitch
        rollPID = new PIDController(1f, 0.1f, 0f); // Tuning parameters for roll
    }

    public void InitEngine()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateEngine(Rigidbody rb, DroneInput input, float maxPower)
    {
        float deltaTime = Time.deltaTime;

        // Get current states
        float currentAltitude = transform.position.y;
        float currentPitch = transform.eulerAngles.x;
        float currentRoll = transform.eulerAngles.z;

        // Calculate the control inputs using PID controllers
        float altitudeControl = altitudePID.Update(targetAltitude, currentAltitude, deltaTime);
        float pitchControl = pitchPID.Update(targetPitch, currentPitch, deltaTime);
        float rollControl = rollPID.Update(targetRoll, currentRoll, deltaTime);

        // Calculate the total engine force based on PID outputs
        Vector3 engineForce = transform.up * (altitudeControl + (input.Throttle * maxPower)) / 4f;

        rb.AddForce(engineForce, ForceMode.Force);

        // Apply shaking effect if enabled
        if (enableShake)
        {
            Vector3 shakeForce = new Vector3(
                Random.Range(-shakeMagnitude, shakeMagnitude),
                Random.Range(-shakeMagnitude, shakeMagnitude),
                Random.Range(-shakeMagnitude, shakeMagnitude)
            );

            rb.AddForce(shakeForce * shakeFrequency, ForceMode.Force);
        }

        HandlePropeller();
    }

    void HandlePropeller() {
        if (!propeller) {
            return;
        }

        propeller.Rotate(Vector3.forward, propRotSpeed);
    }
}
