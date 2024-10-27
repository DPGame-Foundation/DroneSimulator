using UnityEngine;

public class PIDController
{
    private float _kp; // Proportional gain
    private float _ki; // Integral gain
    private float _kd; // Derivative gain

    private float _integral;
    private float _previousError;

    public PIDController(float kp, float ki, float kd)
    {
        _kp = kp;
        _ki = ki;
        _kd = kd;
        _integral = 0f;
        _previousError = 0f;
    }

    public float Update(float target, float current, float deltaTime)
    {
        float error = target - current;
        _integral += error * deltaTime;
        float derivative = (error - _previousError) / deltaTime;
        _previousError = error;

        // PID output
        return _kp * error + _ki * _integral + _kd * derivative;
    }
}
