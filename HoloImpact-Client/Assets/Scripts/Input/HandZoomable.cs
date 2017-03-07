using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Enables scaling of targeted object through manipulation gesture.
/// Requires target to have <c>DescendantsScaler</c>, not just a <c>Transform</c>.
/// </summary>
public class HandZoomable : HandManipulation, IInputBehaviour
{
    [Tooltip("Scale by which object is zoomed on per step.")]
    public float ZoomFactor = 1.5f;

    [Tooltip("Distance which hand should move to trigger next zoom level.")]
    public float ZoomStep = 0.2f;

    [SerializeField]
    private DescendantsScaler m_descendantsScaler;
    private int m_zoomLevel;

    protected override void Start()
    {
        base.Start();
        m_descendantsScaler = TargetTransform.gameObject.GetComponent<DescendantsScaler>();

        if (!m_descendantsScaler)
        {
            enabled = false;
        }
    }

    public void CopySettings(IInputSettings inputSettings)
    {
        var zoomSettings = inputSettings as HandZoomableSettings;
        if (zoomSettings != null)
        {
            TargetTransform = zoomSettings.TargetTransform;
            DistanceScale = zoomSettings.DistanceScale;
            IsManipulationEnabled = zoomSettings.IsZoomingEnabled;
            ZoomFactor = zoomSettings.ZoomFactor;
            ZoomStep = zoomSettings.ZoomStep;

            Start();
        }
    }

    protected override void StartManipulating()
    {
        base.StartManipulating();
        m_zoomLevel = 0;
    }

    protected override void ProcessManipulationPosition()
    {
        var relHandPosition = m_objRefGrabPoint + m_draggingPosition - TargetTransform.position;
        var newZoomLevel = (int)(relHandPosition.x / ZoomStep);

        if (m_zoomLevel != newZoomLevel)
        {
            var zoomScale = (m_zoomLevel < newZoomLevel) ? ZoomFactor : 1 / ZoomFactor;
            m_zoomLevel = newZoomLevel;
            m_descendantsScaler.ScaleBy(Vector3.one * zoomScale);
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
}