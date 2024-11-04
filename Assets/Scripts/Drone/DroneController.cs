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

    [SerializeField] private bool autoInvertControls = true; // New field to enable auto-inversion
    [SerializeField] private float directionChangeThreshold = 0.1f; // Sensitivity for direction change

    private DroneInput input;

    [SerializeField] private DroneEngine frontRightEngine;
    [SerializeField] private DroneEngine frontLeftEngine;
    [SerializeField] private DroneEngine backRightEngine;
    [SerializeField] private DroneEngine backLeftEngine;

    private Vector3 homePosition = new Vector3(0, 1, 0);
    private float pitch, roll, yaw;
    private float currentBoost = 0f;

    [SerializeField] private float BoostDup = 2f;

    // Variables for auto-inversion
    private float lastForwardInput = 0f;
    private bool isPitchInverted = false;
    private bool isRollInverted = false;

    //[SerializeField] private LidarSensor lidarSensor;

    [SerializeField] private IMUSensor imuSensor;

    void Start() 
    {
        input = GetComponent<DroneInput>();
    }

    void Update() 
    {
        float roll = imuSensor.GetRoll();
        float pitch = imuSensor.GetPitch();
        float yaw = imuSensor.GetYaw();

        //Debug.Log("Roll: " + roll + ", Pitch: " + pitch + ", Yaw: " + yaw);

        if (autoInvertControls)
        {
            DetectControlInversion();
        }
    }

    private void DetectControlInversion()
    {
        // Detect pitch (forward/backward) inversion
        if (Mathf.Abs(input.Cyclic.y) > directionChangeThreshold)
        {
            // If input direction changes significantly
            if (Mathf.Sign(input.Cyclic.y) != Mathf.Sign(lastForwardInput))
            {
                // Toggle pitch inversion
                isPitchInverted = !isPitchInverted;
            }
            
            // Update last forward input
            lastForwardInput = input.Cyclic.y;
        }

        // Similar logic could be added for roll if needed
    }

    protected override void HandlePhysics() 
    {
        if (input.goHome) 
        {
            GoHome();
            input.goHome = false;
        }

        if (input.startStop) 
        {
            HandleControls();
        }

        if (input.boost) 
        {
            currentBoost = motorBaseForce * BoostDup;
        } 
        else 
        {
            currentBoost = motorBaseForce;
        }
    }

    private void GoHome() 
    {
        transform.rotation = Quaternion.identity;
        transform.position = homePosition;
    }

    protected virtual void HandleControls() 
    {
        // Apply auto-inversion or manual inversion
        pitch = (autoInvertControls ? (isPitchInverted ? -1 : 1) : 1) 
                * input.Cyclic.y * minMaxPitch;
        
        roll = (autoInvertControls ? (isRollInverted ? -1 : 1) : 1) 
               * -input.Cyclic.x * minMaxRoll;
        
        yaw = input.Pedals * yawPower;
        
        ApplyMotorForces(Mathf.Clamp(input.Throttle, 0f, 1f), pitch, roll, yaw);
    }

    private void ApplyMotorForces(float throttle, float pitch, float roll, float yaw) 
    {
        // Base force from throttle input, modulated by motor base force and throttle level
        float baseForce = currentBoost * throttle;

        // Calculate each motor's force for pitch, roll, and yaw controls
        float frontRightForce = baseForce + (pitch * 0.5f) + (roll * 0.5f);
        float frontLeftForce = baseForce + (pitch * 0.5f) - (roll * 0.5f);
        float backRightForce = baseForce - (pitch * 0.5f) + (roll * 0.5f);
        float backLeftForce = baseForce - (pitch * 0.5f) - (roll * 0.5f);

        // Clamp forces to max power level for each motor
        frontRightForce = Mathf.Clamp(frontRightForce, 0, maxPower);
        frontLeftForce = Mathf.Clamp(frontLeftForce, 0, maxPower);
        backRightForce = Mathf.Clamp(backRightForce, 0, maxPower);
        backLeftForce = Mathf.Clamp(backLeftForce, 0, maxPower);

        Debug.Log("FRF: " + frontRightForce + ", FLF: " + frontLeftForce + ", BRF: " + backRightForce + ", BLF: " + backLeftForce);

        // Apply forces to each engine
        frontRightEngine.UpdateEngine(rb, frontRightForce);
        frontLeftEngine.UpdateEngine(rb, frontLeftForce);
        backRightEngine.UpdateEngine(rb, backRightForce);
        backLeftEngine.UpdateEngine(rb, backLeftForce);
    }
}