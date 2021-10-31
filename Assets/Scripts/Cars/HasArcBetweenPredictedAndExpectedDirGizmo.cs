using UnityEngine;

namespace Cars
{
public class HasArcBetweenPredictedAndExpectedDirGizmo : MonoBehaviour
{
    public Transform predicted;
    public Transform expected;
    public float radius = 5;

    private void OnDrawGizmos()
    {
        if (predicted == null || expected == null) return;
        
        var position = transform.position;
        var expectedDir = (expected.position - position).normalized;
        var predictedDir = (predicted.position - position).normalized;
        GizmosExtensions.DrawWireArcBetweenPredictedAndExpectedDir(position, expectedDir, predictedDir, radius);
    }
}
}