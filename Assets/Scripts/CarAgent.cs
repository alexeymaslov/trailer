using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarAgent : Agent
{
    public Transform target;

    public float trainingAreaWidth = 6;
    public float trainingAreaHeight = 6;

    private Car car;
    public Trailer trailer;
    private Rigidbody trailerRigidbody;
    private Rigidbody rbody;
    private Vector3 offsetBetweenCarAndTrailer;

    private float? oldDistanceXToTarget;
    
    private float maxNegativeY = -0.001f;
    private BehaviorParameters behaviourParameters;

    private void Start()
    {
        behaviourParameters = GetComponent<BehaviorParameters>();
        car = GetComponent<Car>();
        rbody = GetComponent<Rigidbody>();
        offsetBetweenCarAndTrailer = trailer.transform.position - car.transform.position;
        trailerRigidbody = trailer.GetComponent<Rigidbody>();
    }

    private bool ShouldReset()
    {
        if (transform.localPosition.y < maxNegativeY)
        {
            Debug.Log($"Should reset: localPosition.y={transform.localPosition.y} < {maxNegativeY}");
            return true;
        }

        var eulerAngles = transform.localRotation.eulerAngles;
        
        if (Math.Abs(eulerAngles.z) > 20 && Math.Abs(eulerAngles.z) < 340)
        {
            Debug.Log($"Should reset: 20 < abs(eulerAngles.z)={Math.Abs(eulerAngles.z)} < 340");
            return true;
        }

        if (Math.Abs(eulerAngles.x) > 20 && Math.Abs(eulerAngles.x) < 340)
        {
            Debug.Log($"Should reset: 20 < abs(eulerAngles.x)={Math.Abs(eulerAngles.x)} < 340");
            return true;
        }

        return false;
    }

    private void ResetPosition()
    {
        rbody.velocity = Vector3.zero;
        rbody.angularVelocity = Vector3.zero;
        trailerRigidbody.velocity = Vector3.zero;
        trailerRigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 0, 0);
        trailer.transform.localPosition = transform.localPosition + offsetBetweenCarAndTrailer;
        trailer.transform.rotation = Quaternion.identity;
        car.transform.rotation = Quaternion.identity;
    }
    
    public override void OnEpisodeBegin()
    {
        if (ShouldReset())
            ResetPosition();

        oldDistanceXToTarget = null;
        
        target.localPosition = new Vector3(
            Random.value * trainingAreaWidth - trainingAreaHeight / 2,
            0.5f,
            Random.value * trainingAreaHeight - trainingAreaHeight / 2
        );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(rbody.velocity.x);
        sensor.AddObservation(rbody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        car.horizontalInput = controlSignal.x;
        car.verticalInput = controlSignal.z;

        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        var dx = transform.InverseTransformPoint(target.localPosition).x;
        
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if (ShouldReset())
        {
            EndEpisodeOrReset();
        }
        else
        {
            if (oldDistanceXToTarget == null)
            {
                oldDistanceXToTarget = dx;
            }
            else
            {
                if (Math.Abs(dx) > Math.Abs(oldDistanceXToTarget.Value))
                {
                    SetReward(-0.1f);
                }
                
                oldDistanceXToTarget = dx;
            }
        }
    }

    private void EndEpisodeOrReset()
    {
        if (behaviourParameters.BehaviorType == BehaviorType.Default)
        {
            EndEpisode();
        }
        else if (behaviourParameters.BehaviorType == BehaviorType.InferenceOnly || behaviourParameters.BehaviorType == BehaviorType.HeuristicOnly)
        {
            ResetPosition();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}