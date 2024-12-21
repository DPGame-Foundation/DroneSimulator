using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(AudioSource))]
public class DroneEngine : MonoBehaviour
{
    [SerializeField] private Transform propeller;
    [SerializeField] private float propRotSpeed = 300f;
    [SerializeField] private float maxPropRotSpeedMultiplier = 2f; // Maximum multiplier for propeller speed
    [SerializeField] private float maxPitch = 2f; // Maximum pitch for the engine sound
    [SerializeField] private DroneBattery battery; // Reference to the battery

    private AudioSource audioSource;
    private float targetMotorForce = 0f;
    private float currentMotorForce = 0f;
    private float forceSmoothTime = 0.1f; // Smooth time for motor force interpolation
    private float forceVelocity = 0f; // Used for SmoothDamp

    // Nominal voltage for the battery
    [SerializeField] private float nominalVoltage = 12f; // Nominal voltage of the battery

    [SerializeField] private float instabilityFactor = 0.1f; // Adjust this to increase or decrease instability
    [SerializeField] private bool enableInstability = true; // Checkbox to enable or disable instability

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // Ensure the audio loops for continuous sound
    }

    public void InitEngine()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateEngine(Rigidbody rb, float motorForce)
    {
        if (battery.IsBatteryDepleted()) // Check if battery is depleted
        {
            motorForce = 0f; // Disable motor if battery is depleted
            HandleEngineSound(0); // Stop sound if no motor force
        }
        else
        {
            // Get the current voltage
            float currentVoltage = battery.GetCurrentVoltage();
            // Calculate a dynamic scaling factor based on the voltage
            float voltageFactor = currentVoltage / nominalVoltage;

            // Limit the scaling factor to a minimum of 0.1 (10% performance) for realism
            voltageFactor = Mathf.Clamp(voltageFactor, 0.1f, 1f);

            // Scale the motor force dynamically based on the voltage factor
            motorForce *= voltageFactor;

            // Simulate motor instability if enabled
            if (enableInstability)
            {
                float fluctuation = Random.Range(-instabilityFactor, instabilityFactor);
                motorForce += fluctuation; // Apply the instability effect

                // Clamp the motorForce to prevent it from going negative
                motorForce = Mathf.Max(0f, motorForce);
            }
        }

        // Set the target motor force, which will be smoothed in the update
        targetMotorForce = motorForce;

        // Interpolate motor force to make transitions smooth
        currentMotorForce = Mathf.SmoothDamp(currentMotorForce, targetMotorForce, ref forceVelocity, forceSmoothTime);

        battery.UseBatteryPower(currentMotorForce * Time.deltaTime); // Use battery power based on motor force

        //Vector3 engineForce = transform.up * currentMotorForce;
        //rb.AddForceAtPosition(engineForce, propeller.transform.position); // Apply at motor position

        Vector3 engineForce = Vector3.zero;
        engineForce = transform.up * currentMotorForce;

        rb.AddForce(engineForce, ForceMode.Force);
        
        HandlePropeller(currentMotorForce); // Pass motorForce to adjust propeller speed
        HandleEngineSound(currentMotorForce); // Adjust sound based on force
    }

    private void HandlePropeller(float motorForce)
    {
        if (!propeller) return;

        if (motorForce > 0)
        {
            // Create a rotation around the Z-axis based on propRotSpeed and motorForce
            Quaternion rotation = Quaternion.Euler(0, 0, propRotSpeed * motorForce * Time.deltaTime);
            
            // Apply the rotation to the current rotation of the propeller
            propeller.rotation *= rotation; // or propeller.Rotate(rotation.eulerAngles);
        }
    }

    private void HandleEngineSound(float motorForce)
    {
        if (audioSource == null) return;

        if (motorForce > 0.01)
        {
            // Start playing if not already playing
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            // Adjust the pitch based on the motor force
            float pitch = Mathf.Lerp(1f, maxPitch, motorForce / 10f); // Adjust 10f for responsiveness
            audioSource.pitch = Mathf.Clamp(pitch, 1f, maxPitch);

            // Adjust volume based on motor force (optional)
            // audioSource.volume = Mathf.Clamp(motorForce / 10f, 0.2f, 1f); // Set a minimum volume for realism
        }
        else
        {
            // Stop playing if motorForce is zero
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
