using UnityEngine;

/// <summary>
/// Use <c>GetComponent</c> on this to get all Scalers.
/// </summary>
public abstract class Scaler : MonoBehaviour
{
    public abstract void ScaleBy(Vector3 scale);
}