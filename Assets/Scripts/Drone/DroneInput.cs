using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class DroneInput : MonoBehaviour
{
    private Vector2 cyclic;
    private float pedals;
    private float throttle;

    public bool startStop;
    public bool goHome;

    public Vector2 Cyclic { get => cyclic; }
    public float Pedals { get => pedals; }
    public float Throttle { get => throttle; }
    public bool StartStop { get => startStop; }
    public bool GoHome { get => goHome; }

    void Update() { }

    private void OnCyclic(InputValue value) {
        cyclic = value.Get<Vector2>();
    }

    private void OnPedals(InputValue value) {
        pedals = value.Get<float>();
    }

    private void OnThrottle(InputValue value) {
        throttle = value.Get<float>();
    }

    private void OnStartStop(InputValue value) {
        startStop = !startStop;
    }

    private void OnGoHome(InputValue value) {
        goHome = !goHome;
    }
}
