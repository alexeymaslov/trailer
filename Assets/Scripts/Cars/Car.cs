using System;
using UnityEngine;

namespace Cars
{
public class Car : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;

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

    private Rigidbody body;
    private Quaternion initialRotation;
    
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        initialRotation = transform.rotation;
    }

    public bool IsStuck()
    {
        var eulerAngles = transform.rotation.eulerAngles;
        var hasNegativePosition = transform.position.y < -1;
        if (hasNegativePosition)
            Debug.Log($"Car is stuck: t={transform.position.y} < -1");
        
        var tooMuchXAngle = (Mathf.Abs(eulerAngles.x) > 5 && Mathf.Abs(eulerAngles.x) < 355);
        if (tooMuchXAngle)
            Debug.Log($"Car is stuck: 5 < euler.x={Mathf.Abs(eulerAngles.x)} < 355");
        
        var tooMuchZAngle = (Mathf.Abs(eulerAngles.z) > 5 && Mathf.Abs(eulerAngles.z) < 355);
        if (tooMuchZAngle)
            Debug.Log($"Car is stuck: 5 < euler.z={Mathf.Abs(eulerAngles.z)} < 355");
        
        return hasNegativePosition || tooMuchXAngle || tooMuchZAngle;
    }

    public void ResetTo()
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 0, 0);
        transform.rotation = initialRotation;
    }
    
    private void FixedUpdate()
    {
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }

    private void Steer()
    {
        var steeringAngle = maxSteerAngle * horizontalInput;
        frontDriverWheelCollider.steerAngle = steeringAngle;
        frontPassengerWheelCollider.steerAngle = steeringAngle;
    }

    private void Accelerate()
    {
        // if (verticalInput < 0) verticalInput = 0f;

        frontDriverWheelCollider.motorTorque = verticalInput * motorForce;
        frontPassengerWheelCollider.motorTorque = verticalInput * motorForce;
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverWheelCollider, frontDriverWheel);
        UpdateWheelPose(frontPassengerWheelCollider, frontPassengerWheel);
        UpdateWheelPose(rearDriverWheelCollider, rearDriverWheel);
        UpdateWheelPose(rearPassengerWheelCollider, rearPassengerWheel);
    }

    private void UpdateWheelPose(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out var pos, out var quat);

        wheelTransform.position = pos;
        wheelTransform.rotation = quat;
    }
}
}