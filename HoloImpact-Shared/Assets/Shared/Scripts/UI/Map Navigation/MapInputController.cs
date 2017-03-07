using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls map behaviours corresponding to the following modes:
/// <c>Place</c>: Move the map window somewhere else in the room.
/// <c>Drag</c>: Pan the terrain around inside the map window.
/// <c>Zoom</c>: Zoom in on the terrain inside the map window.
/// <c>Scale</c>: Resize the map window.
/// </summary>
public class MapInputController : ModalInputController<MapNavigationState, MapNavigationState>
{
    public Transform mapDragTarget, mapPlaceTarget, mapZoomTarget, mapScaleTarget;
    private HandDraggableSettings m_placeSettings = new HandDraggableSettings(),
                                  m_dragSettings = new HandDraggableSettings();
    private HandZoomableSettings  m_zoomSettings = new HandZoomableSettings(),
                                  m_scaleSettings = new HandZoomableSettings();

    protected override void Awake()
    {
        base.Awake();

        m_placeSettings.TargetTransform = mapPlaceTarget;
        m_placeSettings.IsKeepUpright = true;
        m_placeSettings.IsOrientTowardsUser = false;
        m_placeSettings.IsParallelWhileDragging = false;

        m_dragSettings.TargetTransform = mapDragTarget;
        m_dragSettings.IsKeepUpright = false;
        m_dragSettings.IsParallelWhileDragging = true;

        m_zoomSettings.TargetTransform = mapZoomTarget;
        m_zoomSettings.ZoomFactor = 1.5f;
        m_zoomSettings.ZoomStep = 0.2f;

        m_scaleSettings.TargetTransform = mapScaleTarget;
        m_scaleSettings.ZoomFactor = 1.2f;
        m_scaleSettings.ZoomStep = 0.2f;
    }

    protected override IEnumerable<RegisteredInputs> GetValidInputs()
    {
        return new RegisteredInputs[]
        {
            RegisteredInputs.Drag,
            RegisteredInputs.Zoom
        };
    }

    public void OnTransitionPlacing(bool hasEntered)
    {
        var dragger = GetInput(RegisteredInputs.Drag);
        if (dragger != null && mapPlaceTarget)
        {
            dragger.Enable(hasEntered);

            if (hasEntered)
            {
                dragger.CopySettings(m_placeSettings);
            }
        }
    }

    public void OnTransitionDragging(bool hasEntered)
    {
        var dragger = GetInput(RegisteredInputs.Drag);
        if (dragger != null && mapDragTarget)
        {
            dragger.Enable(hasEntered);

            if (hasEntered)
            {
                dragger.CopySettings(m_dragSettings);
            }
        }
    }

    public void OnTransitionZooming(bool hasEntered)
    {
        var zoomer = GetInput(RegisteredInputs.Zoom);
        if (zoomer != null && mapZoomTarget)
        {
            zoomer.Enable(hasEntered);

            if (hasEntered)
            {
                zoomer.CopySettings(m_zoomSettings);
            }
        }
    }

    public void OnTransitionScaling(bool hasEntered)
    {
        var zoomer = GetInput(RegisteredInputs.Zoom);
        if (zoomer != null && mapScaleTarget)
        {
            zoomer.Enable(hasEntered);

            if (hasEntered)
            {
                zoomer.CopySettings(m_scaleSettings);
            }
        }
    }

    private IEnumerable<KeyValuePair<MapNavigationState, Action<bool>>> GetTransitionActions()
    {
        return new Dictionary<MapNavigationState, Action<bool>>()
        {
            { MapNavigationState.Place, OnTransitionPlacing },
            { MapNavigationState.Drag, OnTransitionDragging },
            { MapNavigationState.Zoom, OnTransitionZooming },
            { MapNavigationState.Scale, OnTransitionScaling }
        };
    }

    protected override IEnumerable<KeyValuePair<MapNavigationState, Action>> GetEnterActions()
    {
        foreach (var kvpair in GetTransitionActions())
        {
            yield return new KeyValuePair<MapNavigationState, Action>(kvpair.Key, () => kvpair.Value(true));
        }
    }

    protected override IEnumerable<KeyValuePair<MapNavigationState, Action>> GetExitActions()
    {
        foreach (var kvpair in GetTransitionActions())
        {
            yield return new KeyValuePair<MapNavigationState, Action>(kvpair.Key, () => kvpair.Value(false));
        }
    }
}