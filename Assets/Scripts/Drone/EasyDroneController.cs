using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(DroneInput))]
public class EasyDroneController : BaseRigidbody
{
    [SerializeField] private float minMaxPitch = 30f;
    [SerializeField] private float minMaxRoll = 30f;
    [SerializeField] private float yawPower = 4f;
    [SerializeField] private float maxPower = 10f;
    [SerializeField] private float motorBaseForce = 9.8f;

    private DroneInput input;

    private Vector3 homePosition = new Vector3(0, 1, 0);
    private float pitch, roll, yaw;
    private float currentBoost = 0f;

    [SerializeField] private float BoostDup = 2f;

    [SerializeField] private IMUSensor imuSensor;

    // List of motors
    [SerializeField] private DroneEngine[] motors;
    private float current_alt;

    void Start()
    {
        input = GetComponent<DroneInput>();
    }

    void Update()
    {
        float alt = imuSensor.GetAltitude();
        Debug.Log("Altitude: " + alt);

        if (input.startStop)
        {
            current_alt = alt;
        }
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
            ApplyThrust();
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
        // Calculate the desired pitch, yaw, and roll based on input
        pitch = input.Cyclic.y * minMaxPitch;
        roll = -input.Cyclic.x * minMaxRoll;
        yaw = input.Pedals * yawPower * Time.deltaTime;

        // Combine the new rotation incrementally with the existing Rigidbody's rotation
        Quaternion deltaRotation = Quaternion.Euler(pitch * Time.deltaTime, yaw, roll * Time.deltaTime);
        Quaternion targetRotation = rb.rotation * deltaRotation;

        // Smoothly rotate the Rigidbody to the target rotation
        rb.MoveRotation(targetRotation);
    }


    private void ApplyThrust()
    {
        float throttle = input.Throttle;
        float thrustForce = currentBoost * throttle;

        thrustForce = math.clamp(thrustForce, 0, maxPower);

        // Apply upward thrust force evenly to all motors
        foreach (var motor in motors)
        {
            motor.UpdateEngine(rb, thrustForce);
        }

        // Debug output
        Debug.Log("Thrust: " + thrustForce);
    }
}
