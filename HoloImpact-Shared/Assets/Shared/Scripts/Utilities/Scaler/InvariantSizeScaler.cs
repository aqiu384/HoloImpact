using UnityEngine;

/// <summary>
/// Preserve global size of transform even if parent scaler is changed.
/// </summary>
public class InvariantSizeScaler : Scaler
{
    public override void ScaleBy(Vector3 scale)
    {
        var inverseScale = new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);
        transform.localScale = Vector3.Scale(transform.localScale, inverseScale);
    }
}