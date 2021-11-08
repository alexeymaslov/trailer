using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Cars
{
public class VisualFindAngleAgent : Agent
{
    public Transform target;
    public Transform rotationPoint;

    public float expected;
    public float predicted;
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        expected = CalculateAngle(rotationPoint, target);
        predicted = actions.ContinuousActions[0];

        AddReward(Mathf.Pow(1 - Mathf.Abs(expected - predicted), 40));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
    }
    
    private static float CalculateAngle(Transform rotationPoint, Transform target)
    {
        var targetPositionRelativeToRotationPointVec3 = rotationPoint.InverseTransformPoint(target.position);
        var targetPositionRelativeToRotationPointVec2 = new Vector2(
            targetPositionRelativeToRotationPointVec3.x,
            targetPositionRelativeToRotationPointVec3.z
        );
        targetPositionRelativeToRotationPointVec2.Normalize();
        
        return Mathf.Rad2Deg * Mathf.Atan2(
            targetPositionRelativeToRotationPointVec2.y,
            targetPositionRelativeToRotationPointVec2.x
        ) / 180 * -1;
    }

    private static Vector3 AngleToDirection(float angle)
    {
        var deg = angle * -1 * 180;
        var rad = Mathf.Deg2Rad * deg;
        var x = Mathf.Cos(rad);
        var z = Mathf.Sin(rad);
        
        return new Vector3(x, 0, z).normalized;
    }
    
    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.blue;
        var predictedTargetPosition = rotationPoint.TransformPoint(AngleToDirection(predicted));
        // Gizmos.DrawSphere(predictedTargetPosition, 0.05f);
        var rotationPointPosition = rotationPoint.position;
        GizmosExtensions.DrawWireArcBetweenPredictedAndExpectedDir(
            rotationPointPosition, 
            (target.position - rotationPointPosition).normalized,
            (predictedTargetPosition - rotationPointPosition).normalized,
            0.6f,
            maxSteps: 0
            );
    }
}
}
