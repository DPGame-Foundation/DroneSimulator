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

    private AudioSource audioSource;

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
        Vector3 engineForce = transform.up * motorForce;
        rb.AddForceAtPosition(engineForce, propeller.transform.position); // Apply at motor position

        if (motorForce > 0) {
            HandlePropeller(motorForce); // Pass motorForce to adjust propeller speed
            HandleEngineSound(motorForce); // Adjust sound based on force
        }
    }

    private void HandlePropeller(float motorForce)
    {
        if (!propeller) return;

        propeller.Rotate(Vector3.forward, propRotSpeed * motorForce  * Time.deltaTime);
    }

    private void HandleEngineSound(float motorForce)
{
    if (audioSource == null) return;

    Debug.Log(motorForce);

    if (motorForce > 1)
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
        //audioSource.volume = Mathf.Clamp(motorForce / 10f, 0.2f, 1f); // Set a minimum volume for realism
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
