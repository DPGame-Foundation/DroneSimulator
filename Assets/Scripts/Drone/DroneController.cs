using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore;

[RequireComponent(typeof(DroneInput))]
public class DroneController : BaseRigidbody
{
    [SerializeField] private float minMaxPitch = 30f;
    [SerializeField] private float minMaxRoll = 30f;
    [SerializeField] private float yawPower = 4f;
    [SerializeField] private float lerpSpeed = 2f;

    [SerializeField] private float maxPower = 4f;

    public bool allowFree = false;

    private DroneInput input;
    private List<IEngine> engines = new List<IEngine>();

    private float finalPitch;
    private float finalRoll;
    private float yaw;
    private float finalYaw;

    void Start()
    {
        input = GetComponent<DroneInput>();
        engines = GetComponentsInChildren<IEngine>().ToList<IEngine>();
    }

    protected override void HandlePhysics() {
        if (input.startStop) {
            HandleEngines();
            HandleControls();
        }
    }

    protected virtual void HandleEngines() {
        //rb.AddForce(Vector3.up * (rb.mass * Physics.gravity.magnitude));
        foreach(IEngine engine in engines) {
            engine.UpdateEngine(rb, input, maxPower);
        }
    }

    protected virtual void HandleControls() {
        float pitch = input.Cyclic.y * minMaxPitch;
        float roll = -input.Cyclic.x * minMaxRoll;
        if (!allowFree) {
            yaw += input.Pedals * yawPower;

            finalPitch = Mathf.Lerp(finalPitch, pitch, Time.deltaTime * lerpSpeed);
            finalRoll = Mathf.Lerp(finalRoll, roll, Time.deltaTime * lerpSpeed);
            finalYaw = Mathf.Lerp(finalYaw, yaw, Time.deltaTime * lerpSpeed);
        } else {
            finalPitch += pitch * Time.deltaTime; // Allow continuous pitch rotation
            finalRoll += roll * Time.deltaTime;   // Allow continuous roll rotation

            // Update yaw based on pedal input
            finalYaw += input.Pedals * yawPower * Time.deltaTime;
        }

        Quaternion rot = Quaternion.Euler(finalPitch, finalYaw, finalRoll);
        rb.MoveRotation(rot);

    }
}