using System;
using UnityEngine;

[Serializable]
public class HandDraggableSettings : IInputSettings
{
    [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
    public Transform TargetTransform;

    [Tooltip("Scale by which hand movement in z is multipled to move the dragged object.")]
    public float DistanceScale = 2f;

    [Tooltip("Should the object be kept upright as it is being dragged?")]
    public bool IsKeepUpright = false;

    [Tooltip("Should the object be oriented towards the user as it is being dragged?")]
    public bool IsOrientTowardsUser = true;

    [Tooltip("Should the object be parallel to reference plane it is being dragged?")]
    public bool IsParallelWhileDragging = false;

    public bool IsDraggingEnabled = true;

    public RegisteredInputs GetInputType()
    {
        return RegisteredInputs.Drag;
    }
}

[RequireComponent(typeof(Collider))]
public class HandDraggablePlaceholder : InputPlaceholder
{
    public HandDraggableSettings settings;

    public override IInputSettings GetInputSettings()
    {
        return settings;
    }
}