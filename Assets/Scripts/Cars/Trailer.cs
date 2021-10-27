using UnityEngine;

namespace Cars
{
public class Trailer : MonoBehaviour
{
    public Car car;
    
    public WheelCollider[] wheelColliders;
    
    public float torque = 10f;

    private Rigidbody body;
    private bool isReversing;
    private Vector3 initialLocalPosition;
    private Quaternion initialRotation;
    
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        initialLocalPosition = transform.localPosition;
        initialRotation = transform.rotation;
    }

    public bool IsStuck()
    {
        var eulerAngles = transform.rotation.eulerAngles;
        var hasNegativePosition = transform.position.y < -1;
        if (hasNegativePosition)
            Debug.Log($"Trailer is stuck: t={transform.position.y} < -1");
        
        var tooMuchXAngle = (Mathf.Abs(eulerAngles.x) > 5 && Mathf.Abs(eulerAngles.x) < 355);
        if (tooMuchXAngle)
            Debug.Log($"Trailer is stuck: 5 < euler.x={Mathf.Abs(eulerAngles.x)} < 355");
        
        var tooMuchZAngle = (Mathf.Abs(eulerAngles.z) > 5 && Mathf.Abs(eulerAngles.z) < 355);
        if (tooMuchZAngle)
            Debug.Log($"Trailer is stuck: 5 < euler.z={Mathf.Abs(eulerAngles.z)} < 355");
        
        return hasNegativePosition || tooMuchXAngle || tooMuchZAngle;
    }

    public void Reset()
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        transform.localPosition = initialLocalPosition;
        transform.rotation = initialRotation;
    }
    
    private void Update()
    {
        // Convert from world space to local space. Anything less than zero in the forward direction is backwards.
        // Stationary will count as forwards.
        isReversing = transform.InverseTransformDirection(body.velocity).z < 0f;

        if (car == null) return;
        
        var input = car.verticalInput;
        var accel = Mathf.Clamp(input, 0f, 1f);
        var brake = Mathf.Clamp(-input, 0f, 1f);
        Throttle(accel, brake);
    }

    private void Throttle(float accel, float brake)
    {
        foreach (var wheelCollider in wheelColliders)
        {
            // Accelerator is applied
            if(accel > 0.0f)
            {
                if(isReversing)
                {
                    // When reversing, accelerator acts as a brake.
                    wheelCollider.motorTorque = 0f;
                }
                else
                {
                    // Apply a little bit of motor torque to help us get rolling
                    wheelCollider.motorTorque = torque;
                    wheelCollider.brakeTorque = 0f;
                }
            }
            else if (brake > 0.0f)
            {
                if(isReversing)
                {
                    // The car is trying to reverse, give it some help.
                    wheelCollider.motorTorque = -torque;
                    wheelCollider.brakeTorque = 0f;
                }
                else
                {
                    // The car is braking!
                    wheelCollider.motorTorque = 0f;
                }
            }
        }
    }
}
}
