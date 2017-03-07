using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

/// <summary>
/// Component that allows dragging of an object with your hand on HoloLens.
/// Dragging is done by calculating the angular delta and z-delta between the current and previous hand positions,
/// and then repositioning the object based on that.
/// </summary>
public class MyHandDraggable : HandManipulation, IInputBehaviour
{
    [Tooltip("Should the object be kept upright as it is being dragged?")]
    public bool IsKeepUpright = false;

    [Tooltip("Should the object be oriented towards the user as it is being dragged?")]
    public bool IsOrientTowardsUser = true;

    [Tooltip("Should the object be parallel to reference plane it is being dragged?")]
    public bool IsParallelWhileDragging = false;

    public void CopySettings(IInputSettings inputSettings)
    {
        var dragSettings = inputSettings as HandDraggableSettings;
        if (dragSettings != null)
        {
            TargetTransform = dragSettings.TargetTransform;
            DistanceScale = dragSettings.DistanceScale;
            IsKeepUpright = dragSettings.IsKeepUpright;
            IsOrientTowardsUser = dragSettings.IsOrientTowardsUser;
            IsParallelWhileDragging = dragSettings.IsParallelWhileDragging;
            IsManipulationEnabled = dragSettings.IsDraggingEnabled;
        }
    }

    public void Enable(bool shouldEnable)
    {
        IsManipulationEnabled = shouldEnable;
    }

    public bool IsEnabled()
    {
        return IsManipulationEnabled;
    }

    public void SetInputController(BaseInputController inputController) { }

    /// <summary>
    /// Update the position of the object being dragged.
    /// </summary>
    protected override void ProcessManipulationPosition()
    {
        if (IsOrientTowardsUser)
        {
            m_draggingRotation = Quaternion.LookRotation(TargetTransform.position - GetHandPivotPosition());
        }
        else
        {
            Vector3 objForward = m_objRefForward; // in world space
            m_draggingRotation = Quaternion.LookRotation(objForward);
        }

        // Apply Final Position
        TargetTransform.position = m_draggingPosition + m_objRefGrabPoint;

        if (IsParallelWhileDragging)
        {
            float rayDistance;
            var cameraPoint = m_mainCamera.transform.position;
            var gazeDragRay = new Ray(cameraPoint, TargetTransform.position - cameraPoint);
            var refPlane = new Plane(transform.up, transform.position);

            if (refPlane.Raycast(gazeDragRay, out rayDistance))
            {
                TargetTransform.position = gazeDragRay.GetPoint(rayDistance);
            }
        }
        else
        {
            TargetTransform.rotation = m_draggingRotation;
        }

        if (IsKeepUpright)
        {
            Quaternion upRotation = Quaternion.FromToRotation(TargetTransform.up, Vector3.up);
            TargetTransform.rotation = upRotation * TargetTransform.rotation;
        }
    }
}