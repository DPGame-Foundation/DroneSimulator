using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DroneEngine : MonoBehaviour, IEngine
{
    [SerializeField] private Transform propeller;
    [SerializeField] private float propRotSpeed = 300f;

    public void InitEngine()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateEngine(Rigidbody rb, DroneInput input, float maxPower)
    {
        //Debug.Log("Running Engine: " + gameObject.name);
        //Debug.Log("Throttle Engine: " + input.Throttle);
        Vector3 upVec = transform.up;
        upVec.x = 0f;
        upVec.z = 0f;
        float diff = 1 - upVec.magnitude;
        float finalDiff = Physics.gravity.magnitude * diff;

        Vector3 engineForce = Vector3.zero;
        engineForce = transform.up * ((rb.mass * Physics.gravity.magnitude + finalDiff) + (input.Throttle * maxPower)) / 4f;

        rb.AddForce(engineForce, ForceMode.Force);

        HandlePropeller();
    }

    void HandlePropeller() {
        if (!propeller) {
            return;
        }

        propeller.Rotate(Vector3.forward, propRotSpeed);
    }
}