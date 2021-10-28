using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cars
{
public class DriveAgent : Agent
{
    public Transform target;

    public float trainingAreaX = 10;
    public float trainingAreaZ = 10;

    public Car car;
    private Rigidbody carBody;
    public Trailer trailer;

    private BehaviorParameters behaviorParameters;
    public Vector3 targetPositionRelativeToCar;
    public float horizontalInputAccumulator;

    private void Start()
    {
        carBody = car.GetComponent<Rigidbody>();
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    public override void OnEpisodeBegin()
    {
        if (ShouldReset())
        {
            Reset();
        }

        target.localPosition = new Vector3(
            Random.value * trainingAreaX - trainingAreaX / 2,
            0.5f,
            Random.value * trainingAreaZ - trainingAreaZ / 2
        );

        targetPositionRelativeToCar = car.transform.InverseTransformPoint(target.position);
        horizontalInputAccumulator = 0;
    }

    private void Reset()
    {
        car.ResetTo();
        if (trailer != null)
            trailer.Reset();
    }

    private bool ShouldReset()
    {
        return car.IsStuck() || trailer != null && trailer.IsStuck();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(car.transform.localPosition);

        sensor.AddObservation(carBody.velocity.x);
        sensor.AddObservation(carBody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var horizontalInput = actions.ContinuousActions[0];
        horizontalInput = Mathf.Clamp(horizontalInput, -1, 1);
        var verticalInput = actions.ContinuousActions[1];
        verticalInput = Mathf.Clamp01(verticalInput);
        
        car.horizontalInput = horizontalInput;
        car.verticalInput = verticalInput;

        horizontalInputAccumulator += horizontalInput;

        float distanceToTarget = Vector3.Distance(
            car.transform.localPosition,
            target.localPosition
        );

        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            EndEpisode();
            // if (targetPositionRelativeToCar.x < 0 && horizontalInputAccumulator > 0)
            // {
            //     SetReward(0.5f);
            //     EndEpisode();
            // }
            // else if (targetPositionRelativeToCar.x > 0 && horizontalInputAccumulator < 0)
            // {
            //     SetReward(0.5f);
            //     EndEpisode();
            // }
            // else
            // {
            //     SetReward(1.0f);
            //     EndEpisode();
            // }
        }
        else
        {
            AddReward(-0.001f);
        }

        if (ShouldReset())
        {
            AddReward(-1.0f);
            if (behaviorParameters.IsInHeuristicMode() || 
                behaviorParameters.BehaviorType == BehaviorType.InferenceOnly)
            {
                Reset();
            }
            else
            {
                EndEpisode();
            }
        }
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.25f, 0f, 0.5f, 0.25f);
        Gizmos.DrawCube(
            transform.TransformPoint(Vector3.zero),
            new Vector3(trainingAreaX, 2, trainingAreaZ)
        );
    }
}
}