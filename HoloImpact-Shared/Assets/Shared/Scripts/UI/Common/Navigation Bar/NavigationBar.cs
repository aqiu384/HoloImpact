using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control navigation bar state.
/// Allows for minimizing, maximizing, and closing.
/// </summary>
[AddComponentMenu("UI/NavigationBar")]
[RequireComponent(typeof(NavigationBarFSM))]
public class NavigationBar : MonoBehaviour
{
    private NavigationBarFSM m_navigationBarFSM;
    private TransformOverTime m_menuTimeTransform;

    public GameObject parentMenu;
    public Button closeButton;

    protected virtual void Awake()
    {
        if (parentMenu == null ||
            closeButton == null)
        {
            Destroy(gameObject);
            return;
        }

        m_navigationBarFSM = GetComponent<NavigationBarFSM>();
        m_menuTimeTransform = parentMenu.AddComponent<TransformOverTime>();
        closeButton.onClick.AddListener(CloseMenu);

        m_navigationBarFSM.AddOnEnterListener(OnMaximizingEnter, NavigationBarState.Maximizing);
        m_navigationBarFSM.AddOnEnterListener(OnMinimizingOrExitingEnter, NavigationBarState.Minimizing);
        m_navigationBarFSM.AddOnEnterListener(OnMinimizingOrExitingEnter, NavigationBarState.Exiting);

        m_navigationBarFSM.AddOnExitListener(OnExitingExit, NavigationBarState.Exiting);
    }

    void Update()
    {
        if (Input.GetKeyDown("="))
        {
            m_navigationBarFSM.OnTransition(NavigationBarTransition.Maximize);
        }
        else if (Input.GetKeyDown("-"))
        {
            m_navigationBarFSM.OnTransition(NavigationBarTransition.Minimize);
        }
    }

    public void CloseMenu()
    {
        m_navigationBarFSM.OnTransition(NavigationBarTransition.Exit);
    }

    public void OnMaximizingEnter()
    {
        m_menuTimeTransform.TransitionTowardsLocal(
            Vector3.up * 0.3f,
            Vector3.one * 0.003f,
            4.0f
        );
    }

    public void OnMinimizingOrExitingEnter()
    {
        m_menuTimeTransform.TransitionTowardsLocal(
            Vector3.zero,
            Vector3.zero,
            4.0f
        );
    }

    public void OnExitingExit()
    {
        Destroy(parentMenu);
    }
}