using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DroneInput))]
public class DroneController : BaseRigidbody
{
    [SerializeField] private float minMaxPitch = 30f;
    [SerializeField] private float minMaxRoll = 30f;
    [SerializeField] private float yawPower = 4f;
    [SerializeField] private float maxPower = 4f;
    [SerializeField] private float motorBaseForce = 9.8f;

    private DroneInput input;

    [SerializeField] private DroneEngine frontRightEngine;
    [SerializeField] private DroneEngine frontLeftEngine;
    [SerializeField] private DroneEngine backRightEngine;
    [SerializeField] private DroneEngine backLeftEngine;

    private Vector3 homePosition = new Vector3(0, 1, 0);
    private float pitch, roll, yaw;

    // New variable for easy mode
    [SerializeField] private bool easyMode = false;

    // New variables for easy mode
    [SerializeField] private float easyModeSpeed = 0.5f;
    [SerializeField] private float easyModeStabilizationFactor = 0.2f;

    void Start()
    {
        input = GetComponent<DroneInput>();
    }

    protected override void HandlePhysics()
    {
        if (input.goHome)
        {
            GoHome();
            input.goHome = false;
        }
        else if (input.startStop)
        {
            HandleControls();
        }
    }

    private void GoHome()
    {
        transform.rotation = Quaternion.identity;
        transform.position = homePosition;
    }

    protected virtual void HandleControls()
    {
        // If in easy mode, simplify controls
        if (easyMode)
        {
            // Use throttle to control altitude
            float throttle = Mathf.Clamp(input.Throttle, 0f, 1f);
            float targetHeight = Mathf.Lerp(transform.position.y, homePosition.y, easyModeStabilizationFactor);
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);

            // Simple forward/backward control
            pitch = input.Cyclic.y * minMaxPitch * easyModeSpeed;
            roll = -input.Cyclic.x * minMaxRoll * easyModeSpeed;

            // Stabilize yaw
            yaw = 0f;
        }
        else
        {
            // Normal controls
            pitch = input.Cyclic.y * minMaxPitch;
            roll = -input.Cyclic.x * minMaxRoll;
            yaw = input.Pedals * yawPower;
        }

        ApplyMotorForces(Mathf.Clamp(input.Throttle, 0f, 1f), pitch, roll, yaw);
    }

    private void ApplyMotorForces(float throttle, float pitch, float roll, float yaw)
    {
        // Base force from throttle input, modulated by motor base force and throttle level
        float baseForce = motorBaseForce * throttle;

        // Calculate each motor's force for pitch, roll, and yaw controls
        float frontRightForce = baseForce + (pitch * 0.5f) + (roll * 0.5f) - (yaw * 0.5f);
        float frontLeftForce = baseForce + (pitch * 0.5f) - (roll * 0.5f) + (yaw * 0.5f);
        float backRightForce = baseForce - (pitch * 0.5f) + (roll * 0.5f) + (yaw * 0.5f);
        float backLeftForce = baseForce - (pitch * 0.5f) - (roll * 0.5f) - (yaw * 0.5f);

        // Clamp forces to max power level for each motor
        frontRightForce = Mathf.Clamp(frontRightForce, 0, maxPower);
        frontLeftForce = Mathf.Clamp(frontLeftForce, 0, maxPower);
        backRightForce = Mathf.Clamp(backRightForce, 0, maxPower);
        backLeftForce = Mathf.Clamp(backLeftForce, 0, maxPower);

        // Apply forces to each engine
        frontRightEngine.UpdateEngine(rb, frontRightForce);
        frontLeftEngine.UpdateEngine(rb, frontLeftForce);
        backRightEngine.UpdateEngine(rb, backRightForce);
        backLeftEngine.UpdateEngine(rb, backLeftForce);
    }
}
