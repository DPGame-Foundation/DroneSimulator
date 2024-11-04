using UnityEngine;
using System.Collections.Generic;

public class LidarSensor : MonoBehaviour
{
    public int numberOfRays = 360; // Number of rays to cast
    public float angleRange = 360f;
    public float maxDistance = 100f; // Maximum distance of rays
    public LayerMask targetLayer; // Layer to detect
    public Color hitColor = Color.red; // Color when ray hits an object
    public Color noHitColor = Color.green; // Color when ray does not hit anything
    public bool showDebugLines = true; // Checkbox to turn on/off DrawLine visualization
    private int originalLayer; // To store the original layer of the LIDAR object

    // Stores distances of detected objects for each angle
    public List<float> distances = new List<float>();

    void Start()
    {
        originalLayer = gameObject.layer;

        // Initialize the distances list to hold the number of rays
        for (int i = 0; i < numberOfRays; i++)
        {
            distances.Add(maxDistance);
        }
    }

    void Update()
    {
        CastLidarRays();
    }

    void CastLidarRays()
    {
        // Temporarily change the LIDAR object's layer to ignore it in the raycast
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        // Get the rotation of the current object
        Quaternion rotation = transform.rotation;

        for (int i = 0; i < numberOfRays; i++)
        {
            // Calculate angle in radians
            float angle = i * (angleRange / numberOfRays) * Mathf.Deg2Rad;

            // Calculate direction using cosine and sine, applying the object's rotation
            Vector3 direction = rotation * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            // Cast the ray
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, maxDistance, targetLayer))
            {
                // Store the distance of the hit
                distances[i] = hit.distance;

                // Optionally draw a line to the hit point
                if (showDebugLines)
                {
                    Debug.DrawLine(transform.position, hit.point, hitColor);
                }
            }
            else
            {
                // Set distance to max if no hit
                distances[i] = maxDistance;

                // Optionally draw a line to the maximum range
                if (showDebugLines)
                {
                    Debug.DrawLine(transform.position, transform.position + direction * maxDistance, noHitColor);
                }
            }
        }

        // Revert the LIDAR object's layer to its original
        gameObject.layer = originalLayer;
    }
}
