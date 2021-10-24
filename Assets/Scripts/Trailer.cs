using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trailer : MonoBehaviour
{
    public WheelCollider[] wheelColliders;
    [SerializeField] private float torque = 10f;

    private new Rigidbody rigidbody;
    private bool isReversing;

    public float? verticalInput;
    
    private void Start() => rigidbody = GetComponent<Rigidbody>();

    private void Update()
    {
        // Convert from world space to local space. Anything less than zero in the forward direction is backwards.
        // Stationary will count as forwards.
        isReversing = transform.InverseTransformDirection(rigidbody.velocity).z < 0f;

        // Make it move!
        if (verticalInput == null) return;
        
        float input = Input.GetAxis("Vertical");
        float accel = Mathf.Clamp(input, 0f, 1f);
        float brake = Mathf.Clamp(-input, 0f, 1f);
        Throttle(accel, brake);
    }

    private void Throttle(float accel, float brake)
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            // Accelerator is applied
            if(accel > 0.0f)
            {
                if(isReversing)
                {
                    // When reversing, accelerator acts as a brake.
                    wheelColliders[i].motorTorque = 0f;
                }
                else
                {
                    // Apply a little bit of motor torque to help us get rolling
                    wheelColliders[i].motorTorque = torque;
                    wheelColliders[i].brakeTorque = 0f;
                }
            }
            else if (brake > 0.0f)
            {
                if(isReversing)
                {
                    // The car is trying to reverse, give it some help.
                    wheelColliders[i].motorTorque = -torque;
                    wheelColliders[i].brakeTorque = 0f;
                }
                else
                {
                    // The car is braking!
                    wheelColliders[i].motorTorque = 0f;
                }
            }
        }
    }
}
