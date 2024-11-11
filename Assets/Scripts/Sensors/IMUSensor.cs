using UnityEngine;

public class IMUSensor : MonoBehaviour
{
    // Public properties to access IMU data
    public Vector3 CurrentPosition { get; private set; }
    public Quaternion CurrentRotation { get; private set; }
    public Vector3 EulerAngles { get; private set; }

    private Quaternion lastRotation;
    private Vector3 angularVelocity;

    // Update is called once per frame
    void Update()
    {
        // Update the IMU data
        CurrentPosition = transform.position;
        CurrentRotation = transform.rotation;
        EulerAngles = ConvertToMinus180To180(CurrentRotation.eulerAngles);

        // Calculate angular velocity
        CalculateAngularVelocity();
    }

    // Function to get the current position of the drone
    public Vector3 GetPosition()
    {
        return CurrentPosition;
    }

    // Function to get the current rotation of the drone as a quaternion
    public Quaternion GetRotation()
    {
        return CurrentRotation;
    }

    // Function to get the current rotation in Euler angles
    public Vector3 GetEulerAngles()
    {
        return EulerAngles;
    }

    // Function to get the drone's roll angle (Euler X) in -180 to +180 range
    public float GetRoll()
    {
        return EulerAngles.x;
    }

    // Function to get the drone's pitch angle (Euler Y) in -180 to +180 range
    public float GetPitch()
    {
        return EulerAngles.y;
    }

    // Function to get the drone's yaw angle (Euler Z) in -180 to +180 range
    public float GetYaw()
    {
        return -EulerAngles.z;
    }

    // Function to get the drone's pitch rate (angular velocity around Y axis)
    public float GetPitchRate()
    {
        return angularVelocity.y;
    }

    // Function to get the drone's roll rate (angular velocity around X axis)
    public float GetRollRate()
    {
        return angularVelocity.x;
    }

    // Function to get the drone's yaw rate (angular velocity around Z axis)
    public float GetYawRate()
    {
        return angularVelocity.z;
    }

    public float GetAltitude()
    {
        return CurrentPosition.y; // Altitude is the y-coordinate of the position
    }

    // Helper function to convert Euler angles to -180 to +180 range
    private Vector3 ConvertToMinus180To180(Vector3 euler)
    {
        euler.x = (euler.x > 180) ? euler.x - 360 : euler.x;
        euler.y = (euler.y > 180) ? euler.y - 360 : euler.y;
        euler.z = (euler.z > 180) ? euler.z - 360 : euler.z;
        return euler;
    }

    // Calculate the angular velocity based on the change in rotation
    private void CalculateAngularVelocity()
    {
        // Calculate the difference in rotation from the last frame
        Quaternion deltaRotation = Quaternion.Inverse(lastRotation) * CurrentRotation;
        Vector3 deltaEuler = ConvertToMinus180To180(deltaRotation.eulerAngles);

        // Store angular velocity as the change in angles over the frame time
        angularVelocity = deltaEuler / Time.deltaTime;

        // Update last rotation for the next calculation
        lastRotation = CurrentRotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        lastRotation = CurrentRotation;
    }
}
