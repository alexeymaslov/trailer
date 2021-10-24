using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class AngleEstimatorAgent2 : Agent
{
    public Transform target;
    public Transform rotationPoint;

    public float realAngle;
    public float previousRealAngle;
    public float predictedAngle;
    public float previousPredictedAngle;
    public Vector3 t3;
    public Vector2 t2;
    public Vector2 t2N;
    
    public override void OnEpisodeBegin()
    {
        CalculateRealAngleInDegrees();
        predictedAngle = 0.5f;
        previousPredictedAngle = 0.5f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var targetPositionRelativeToRotationPoint = rotationPoint.InverseTransformPoint(target.position);
        sensor.AddObservation(targetPositionRelativeToRotationPoint.x);
        sensor.AddObservation(targetPositionRelativeToRotationPoint.z);
    }

    private void MaybeEndEposide()
    {
        if (Random.value >= 0)
        {
            EndEpisode();
        }
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        previousPredictedAngle = predictedAngle;
        predictedAngle = actions.ContinuousActions[0];

        CalculateRealAngleInDegrees();

        var realD = realAngle - previousRealAngle;
        var predictedD = predictedAngle - previousPredictedAngle;
        if (Mathf.Sign(predictedD) != Math.Sign(realD))
        {
            SetReward(-1);
            MaybeEndEposide();
        }
        
        var d = Math.Abs(realAngle - predictedAngle);
        if (d < 0.01)
        {
            SetReward(1);
            MaybeEndEposide();
        }
        else if (d < 0.1)
        {
            SetReward(0.5f);
            MaybeEndEposide();
        }
        // else if (d < 0.2)
        // {
        //     SetReward(0.25f);
        //     MaybeEndEposide();
        // }
        // else if (d < 0.3)
        // {
        //     SetReward(0.125f);
        //     MaybeEndEposide();
        // }
        // else if (d < 0.4)
        // {
        //     SetReward(0.0625f);
        //     MaybeEndEposide();
        // }
        // else if (d < 0.5)
        // {
        //     SetReward(0.03f);
        //     MaybeEndEposide();
        // }
    }

    private void CalculateRealAngleInDegrees()
    {
        previousRealAngle = realAngle;
        t3 = rotationPoint.InverseTransformPoint(target.position);
        t2 = new Vector2(t3.x, t3.z);
        t2N = t2.normalized;
        realAngle = Mathf.Rad2Deg * Mathf.Atan2(
            t2N.y,
            t2N.x
        ) / 180 * -1;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        CalculateRealAngleInDegrees();
    }
}
