using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Draws a bubble around object when gaze if focused on it.
/// </summary>
public class Hitbox : MonoBehaviour, IInputBehaviour, IFocusable
{
    private MeshRenderer m_meshRenderer;
    private IGazeInputController m_gazeController;

    public void CopySettings(IInputSettings inputSettings) { }

    public void Enable(bool shouldEnable)
    {
        enabled = shouldEnable;
    }

    public bool IsEnabled()
    {
        return enabled;
    }

    public void SetInputController(BaseInputController inputController)
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_gazeController = inputController as IGazeInputController;
        enabled = true;
        OnFocusExit();
    }

    public void OnFocusEnter()
    {
        m_meshRenderer.enabled = true;
        if (m_gazeController != null) m_gazeController.OnFocusEnter();
    }

    public void OnFocusExit()
    {
        m_meshRenderer.enabled = false;
        if (m_gazeController != null) m_gazeController.OnFocusExit();
    }
}