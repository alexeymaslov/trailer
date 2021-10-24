using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AngleEstimatorAgent : Agent
{
    public Transform target;
    public Transform rotationPoint;

    public float realAngle;
    public float predictedAngle;
    public float reward;

    public override void CollectObservations(VectorSensor sensor)
    {
        var targetPositionRelativeToRotationPoint = rotationPoint.InverseTransformPoint(target.position);
        sensor.AddObservation(targetPositionRelativeToRotationPoint.x);
        sensor.AddObservation(targetPositionRelativeToRotationPoint.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        predictedAngle = actions.ContinuousActions[0];

        UpdateRealAngle();

        var absDifference = Math.Abs(realAngle - predictedAngle);
        if (absDifference > 1)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            reward = (1 - absDifference);
            SetReward(reward);
            EndEpisode();
        }
    }

    private void UpdateRealAngle()
    {
        var targetPositionRelativeToRotationPointVec3 = rotationPoint.InverseTransformPoint(target.position);
        var targetPositionRelativeToRotationPointVec2 = new Vector2(
            targetPositionRelativeToRotationPointVec3.x,
            targetPositionRelativeToRotationPointVec3.z
        );
        var targetPositionRelativeToRotationPointVec2Normalized =
            targetPositionRelativeToRotationPointVec2.normalized;
        realAngle = Mathf.Rad2Deg * Mathf.Atan2(
            targetPositionRelativeToRotationPointVec2Normalized.y,
            targetPositionRelativeToRotationPointVec2Normalized.x
        ) / 180 * -1;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
    }
}