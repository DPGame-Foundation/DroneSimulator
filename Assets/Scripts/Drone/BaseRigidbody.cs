using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseRigidbody : MonoBehaviour
{
    [SerializeField] private float weightInKg = 1f;

    protected Rigidbody rb;
    protected float startDrag;
    protected float startAngularDrag;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb) {
            rb.mass = weightInKg;
            startDrag = rb.linearDamping;
            startAngularDrag = rb.angularDamping;
        }
    }

    void FixedUpdate() {
        if (!rb) {
            return;
        }
        
        HandlePhysics();
    }

    protected virtual void HandlePhysics() { }
}
