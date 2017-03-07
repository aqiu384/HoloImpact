using UnityEngine;

/// <summary>
/// Find all <c>Scaler</c> descendents and notifies them about scale change.
/// </summary>
public class DescendantsScaler : MonoBehaviour
{
    public void ScaleBy(Vector3 scale)
    {
        transform.localScale = Vector3.Scale(transform.localScale, scale);

        foreach (var child in GetComponentsInChildren<Scaler>())
        {
            child.ScaleBy(scale);
        }
    }
}