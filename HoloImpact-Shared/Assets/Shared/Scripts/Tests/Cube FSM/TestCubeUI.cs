using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Synchronizes state between Network Connection menu and underlying Network Connection FSM.
/// </summary>
public class TestCubeUI : MonoBehaviour
{
    public TestCubeFSM testCubeFSM;
    public GameObject minimizedSubmenu, growingSubmenu, maximizedSubmenu, shrinkingSubmenu;

    private bool CheckPreconditions()
    {
        var submenus = new Dictionary<TestCubeState, GameObject>()
        {
            { TestCubeState.Minimized, minimizedSubmenu },
            { TestCubeState.Growing, growingSubmenu },
            { TestCubeState.Maximized, maximizedSubmenu },
            { TestCubeState.Shrinking, shrinkingSubmenu },
        };

        foreach (var kvpair in submenus)
        {
            var state = kvpair.Key;
            var submenu = kvpair.Value;

            testCubeFSM.AddOnEnterListener(() => submenu.SetActive(true), state);
            testCubeFSM.AddOnExitListener(() => submenu.SetActive(false), state);
            submenu.SetActive(false);
        }

        return true;
    }

    protected virtual void Start()
    {
        CheckPreconditions();
    }

    public void StartGrowing()
    {
        testCubeFSM.OnTransition(TestCubeTransition.StartGrowing);
    }

    public void StartShrinking()
    {
        testCubeFSM.OnTransition(TestCubeTransition.StartShrinking);
    }
}