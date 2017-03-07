using UnityEngine;
using System;

[Serializable]
public class HandRotatableSettings : IInputSettings
{
    [Tooltip("Transform that will be rotated on. Defaults to the object of the component.")]
    public Transform TargetTransform;

    [Tooltip("Scale by which hand movement in z is multipled to move the dragged object.")]
    public float DistanceScale = 2f;

    public bool IsRotatingEnabled = true;

    public RegisteredInputs GetInputType()
    {
        return RegisteredInputs.Zoom;
    }
}

[RequireComponent(typeof(Collider))]
public class HandRotatablePlaceholder : InputPlaceholder
{
    public HandRotatableSettings settings;

    public override IInputSettings GetInputSettings()
    {
        return settings;
    }
}