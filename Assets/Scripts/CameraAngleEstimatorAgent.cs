using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CameraAngleEstimatorAgent : Agent
{
    public Transform target;
    public Transform rotationPoint;

    public float angleInCollectObservations;
    public float realAngle;
    public float predictedAngle;
    public float reward;

    public override void CollectObservations(VectorSensor sensor)
    {
        angleInCollectObservations = CalculateAngle(rotationPoint, target);
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
            reward = Mathf.Pow(1 - absDifference, 40);
            SetReward(reward);
            EndEpisode();
        }
    }

    private void UpdateRealAngle()
    {
        realAngle = CalculateAngle(rotationPoint, target);
    }

    private static float CalculateAngle(Transform rotationPoint, Transform target)
    {
        var targetPositionRelativeToRotationPointVec3 = rotationPoint.InverseTransformPoint(target.position);
        var targetPositionRelativeToRotationPointVec2 = new Vector2(
            targetPositionRelativeToRotationPointVec3.x,
            targetPositionRelativeToRotationPointVec3.z
        );
        var targetPositionRelativeToRotationPointVec2Normalized =
            targetPositionRelativeToRotationPointVec2.normalized;
        return Mathf.Rad2Deg * Mathf.Atan2(
            targetPositionRelativeToRotationPointVec2Normalized.y,
            targetPositionRelativeToRotationPointVec2Normalized.x
        ) / 180 * -1;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
    }
}