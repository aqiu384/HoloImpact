using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// /// Synchronizes state between Vehicle Selection menu and the underlying Vehicle Selection FSM
/// </summary>
public class VehicleSelectorMenu : MonoBehaviour
{
    public GameObject defaultSubmenu, waitingSubmenu, processingSubmenu;
    public VehicleSelectorFSM stateMachine;

    private IDictionary<VehicleSelectorState, GameObject> m_submenus;

    protected virtual void Start()
    {
        if (!defaultSubmenu ||
            !waitingSubmenu ||
            !processingSubmenu ||
            !stateMachine)
        {
            enabled = false;
            return;
        }

        m_submenus = new Dictionary<VehicleSelectorState, GameObject>()
        {
            { VehicleSelectorState.Default, defaultSubmenu },
            { VehicleSelectorState.JumpTo, waitingSubmenu },
            { VehicleSelectorState.Track, waitingSubmenu },
            { VehicleSelectorState.Info, waitingSubmenu },
            { VehicleSelectorState.Processing, processingSubmenu }
        };

        foreach (var kvpair in m_submenus)
        {
            Action showMenu = () => kvpair.Value.SetActive(true);
            Action hideMenu = () => kvpair.Value.SetActive(false);

            stateMachine.AddOnEnterListener(showMenu, kvpair.Key);
            stateMachine.AddOnExitListener(hideMenu, kvpair.Key);

            hideMenu();
        }
    }

    public void StartJumpTo()
    {
        stateMachine.OnTransition(VehicleSelectorTransition.JumpTo);
    }

    public void StartTrack()
    {
        stateMachine.OnTransition(VehicleSelectorTransition.Track);
    }

    public void StartInfo()
    {
        stateMachine.OnTransition(VehicleSelectorTransition.Info);
    }

    public void CancelSelection()
    {
        stateMachine.OnTransition(VehicleSelectorTransition.Cancel);
    }
}