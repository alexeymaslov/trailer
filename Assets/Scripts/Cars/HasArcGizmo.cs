using UnityEngine;

namespace Cars
{
public class HasArcGizmo : MonoBehaviour
{
    public float anglesRange = 30;
    public float radius = 5;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosExtensions.DrawWireArc(transform.position, Vector3.forward, anglesRange, radius);
    }
}

public static class GizmosExtensions
{
    // returns angle <- [0, 360)
    private static float MapDegreeTo0360(float angle)
    {
        while (angle < 0)
            angle += 360;

        while (angle >= 360)
            angle -= 360;

        return angle;
    }

    // private static bool IsIn180RangeCounterClockWise(float from, float to)
    // {
    //     from = MapDegreeTo0360(from);
    //     to = MapDegreeTo0360(to);
    //
    //     // from + d = to
    //     var d = to - from;
    //     if (to < 180)
    //     {
    //         return to > from && to < MapDegreeTo0360(from - 180);
    //     }
    //     else
    //     {
    //         return to > from || to < MapDegreeTo0360(from - 180);
    //     }
    // }
    
    public static void DrawWireArcBetweenPredictedAndExpectedDir(Vector3 position,
        Vector3 expectedDir,
        Vector3 predictedDir,
        float radius,
        Color? expectedColor = null,
        Color? predictedColor = null,
        float maxSteps = 20)
    {
        expectedColor ??= Color.green;
        predictedColor ??= Color.yellow;

        Gizmos.color = expectedColor.Value;
        var expectedDirAngle = MapDegreeTo0360(GetAnglesFromDir(position, expectedDir));
        Debug.Log($"expectedDirAngle={expectedDirAngle}");
        var predictedDirAngle = MapDegreeTo0360(GetAnglesFromDir(position, predictedDir));
        Debug.Log($"predictedDirAngle={predictedDirAngle}");

        var expectedDirPosition = position + 
                          new Vector3(
                              radius * Mathf.Cos(Mathf.Deg2Rad * expectedDirAngle), 
                              0, 
                              radius * Mathf.Sin(Mathf.Deg2Rad * expectedDirAngle)
                              );
        Gizmos.color = expectedColor.Value;
        Gizmos.DrawLine(position, expectedDirPosition);

        var predictedDirPosition = position + 
                                   new Vector3(
                                       radius * Mathf.Cos(Mathf.Deg2Rad * predictedDirAngle), 
                                       0, 
                                       radius * Mathf.Sin(Mathf.Deg2Rad * predictedDirAngle)
                                   );
        Gizmos.color = predictedColor.Value;
        Gizmos.DrawLine(position, predictedDirPosition);
        
        Vector3 arcSegmentStart;
        float fromAngle;
        float toAngle;
        float angleStep;
        if (expectedDirAngle < predictedDirAngle)
        {
            fromAngle = expectedDirAngle;
            toAngle = predictedDirAngle;
            arcSegmentStart = expectedDirPosition;
            angleStep = (predictedDirAngle - expectedDirAngle) / maxSteps;
        }
        else
        {
            fromAngle = predictedDirAngle;
            toAngle = expectedDirAngle;
            arcSegmentStart = predictedDirPosition;
            angleStep = (expectedDirAngle - predictedDirAngle) / maxSteps;
        }

        var angle = fromAngle + angleStep;
        for (var i = 0; i < maxSteps; i++)
        {
            
            var arcSegmentEnd = position + 
                                       new Vector3(
                                           radius * Mathf.Cos(Mathf.Deg2Rad * angle), 
                                           0, 
                                           radius * Mathf.Sin(Mathf.Deg2Rad * angle)
                                       );
            Gizmos.color = predictedColor.Value;
            Gizmos.DrawLine(arcSegmentStart, arcSegmentEnd);
            angle += angleStep;
            arcSegmentStart = arcSegmentEnd;
        }
    }

    /// <summary>
    /// Draws a wire arc.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dir">The direction from which the anglesRange is taken into account</param>
    /// <param name="anglesRange">The angle range, in degrees.</param>
    /// <param name="radius"></param>
    /// <param name="maxSteps">How many steps to use to draw the arc.</param>
    public static void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, float maxSteps = 20)
    {
        var srcAngles = GetAnglesFromDir(position, dir);
        var initialPos = position;
        var posA = initialPos;
        var stepAngles = anglesRange / maxSteps;
        var angle = srcAngles - anglesRange / 2;
        for (var i = 0; i <= maxSteps; i++)
        {
            var rad = Mathf.Deg2Rad * angle;
            var posB = initialPos;
            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

            Gizmos.DrawLine(posA, posB);

            angle += stepAngles;
            posA = posB;
        }

        Gizmos.DrawLine(posA, initialPos);
    }

    static float GetAnglesFromDir(Vector3 position, Vector3 dir)
    {
        var forwardLimitPos = position + dir;
        var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

        return srcAngles;
    }
}
}