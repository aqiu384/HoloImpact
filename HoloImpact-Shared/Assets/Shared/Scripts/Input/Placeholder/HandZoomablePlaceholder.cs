using UnityEngine;
using System;

[Serializable]
public class HandZoomableSettings : IInputSettings
{
    [Tooltip("Transform that will be zoomed on. Defaults to the object of the component.")]
    public Transform TargetTransform;

    [Tooltip("Scale by which hand movement in z is multipled to move the dragged object.")]
    public float DistanceScale = 2f;

    [Tooltip("Scale by which object is zoomed on per step.")]
    public float ZoomFactor = 1.5f;

    [Tooltip("Distance which hand should move to trigger next zoom level.")]
    public float ZoomStep = 0.2f;

    public bool IsZoomingEnabled = true;

    public RegisteredInputs GetInputType()
    {
        return RegisteredInputs.Zoom;
    }
}

[RequireComponent(typeof(Collider))]
public class HandZoomablePlaceholder : InputPlaceholder
{
    public HandZoomableSettings settings;

    public override IInputSettings GetInputSettings()
    {
        return settings;
    }
}