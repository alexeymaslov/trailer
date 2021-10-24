using System;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float? horizontalInput;
    public float? verticalInput;
    private float steeringAngle;

    public WheelCollider frontDriverWheelCollider;
    public WheelCollider frontPassengerWheelCollider;
    public WheelCollider rearDriverWheelCollider;
    public WheelCollider rearPassengerWheelCollider;
    public Transform frontDriverWheel;
    public Transform frontPassengerWheel;
    public Transform rearDriverWheel;
    public Transform rearPassengerWheel;
    public float maxSteerAngle = 30;
    public float motorForce = 50;

    public WheelCollider[] otherWheelColliders;
    public Transform[] otherWheels;

    public Rigidbody rbody;

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }

    private void Steer()
    {
        if (horizontalInput == null) return;
        
        steeringAngle = maxSteerAngle * horizontalInput.Value;
        frontDriverWheelCollider.steerAngle = steeringAngle;
        frontPassengerWheelCollider.steerAngle = steeringAngle;
    }

    private void Accelerate()
    {
        if (verticalInput == null) return;

        if (verticalInput.Value < 0) verticalInput = 0f;
        
        frontDriverWheelCollider.motorTorque = verticalInput.Value * motorForce;
        frontPassengerWheelCollider.motorTorque = verticalInput.Value * motorForce;
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverWheelCollider, frontDriverWheel);
        UpdateWheelPose(frontPassengerWheelCollider, frontPassengerWheel);
        UpdateWheelPose(rearDriverWheelCollider, rearDriverWheel);
        UpdateWheelPose(rearPassengerWheelCollider, rearPassengerWheel);

        if (otherWheels != null && otherWheelColliders != null)
        {
            for (int i = 0; i < otherWheels.Length; i++)
            {
                UpdateWheelPose(otherWheelColliders[i], otherWheels[i]);
            }
        }
    }

    private void UpdateWheelPose(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos = wheelTransform.position;
        Quaternion quat = wheelTransform.rotation;
        
        wheelCollider.GetWorldPose(out pos, out quat);

        wheelTransform.position = pos;
        wheelTransform.rotation = quat;
    }
}
