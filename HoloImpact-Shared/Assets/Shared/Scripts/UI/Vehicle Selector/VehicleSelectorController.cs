using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Waits for selection of vehicle on map and applies one of the following modes:
/// <c>Default</c>: Display a bubble around the selected vehicle.
/// <c>JumpTo</c>: Centers map around selected vehicle.
/// <c>Track</c>: Unimplemented.
/// <c>Info</c>: Spawns informational popup that follows selected vehicle.
/// Once a vehicle is selected the mode is reset to <c>Default</c>.
/// </summary>
[RequireComponent(typeof(VehicleSelectorFSM))]
public class VehicleSelectorController : SelectorController<Vehicle>
{
    public float jumpToSpeed = 10.0f;
    public TransformOverTime mapDragOverTime;

    private VehicleSelectorFSM m_stateMachine;

    [SerializeField]
    private VehicleSelectorState m_selectorState;
    private Vehicle m_selectedVehicle;
    private IDictionary<VehicleSelectorState, Action<Vehicle>> m_selectorActions;

    protected virtual void Awake()
    {
        if (!mapDragOverTime)
        {
            enabled = false;
        }

        m_stateMachine = GetComponent<VehicleSelectorFSM>();

        m_selectorActions = new Dictionary<VehicleSelectorState, Action<Vehicle>>
        {
            { VehicleSelectorState.Default, ProcessDefault },
            { VehicleSelectorState.JumpTo, ProcessJumpTo },
            { VehicleSelectorState.Track, ProcessTrack },
            { VehicleSelectorState.Info, ProcessInfo }
        };

        foreach (var kvpair in m_selectorActions)
        {
            Action setSelector = () => m_selectorState = kvpair.Key;
            m_stateMachine.AddOnEnterListener(setSelector, kvpair.Key);
        }

        m_stateMachine.AddOnEnterListener(OnProcessVehicle, VehicleSelectorState.Processing);
    }

    public override void SelectItem(Vehicle vehicle)
    {
        m_selectedVehicle = vehicle;
        m_stateMachine.OnTransition(VehicleSelectorTransition.VehicleSelected);
    }

    public void OnProcessVehicle()
    {
        m_selectorActions[m_selectorState](m_selectedVehicle);
        m_stateMachine.OnTransition(VehicleSelectorTransition.FinishedProcessing);
    }

    public void ProcessDefault(Vehicle vehicle)
    {
        Debug.Log(string.Format("Vehicle ID {0} was selected while in state Default.", m_selectedVehicle.GetID()));
    }

    public void ProcessJumpTo(Vehicle vehicle)
    {
        var vehicleCenter = -1 * vehicle.transform.localPosition;
        vehicleCenter.y = 0;

        mapDragOverTime.TransitionTowardsLocal(vehicleCenter, jumpToSpeed);
        Debug.Log(string.Format("Vehicle ID {0} was selected while in state JumpTo.", m_selectedVehicle.GetID()));
    }

    public void ProcessTrack(Vehicle vehicle)
    {
        Debug.Log(string.Format("Vehicle ID {0} was selected while in state Track.", m_selectedVehicle.GetID()));
    }

    public void ProcessInfo(Vehicle vehicle)
    {
        var infoMenuCanvas = vehicle.GetComponentInChildren<Canvas>();
        if (infoMenuCanvas)
        {
            infoMenuCanvas.enabled = true;
        }

        Debug.Log(string.Format("Vehicle ID {0} was selected while in state Info.", m_selectedVehicle.GetID()));
    }
}