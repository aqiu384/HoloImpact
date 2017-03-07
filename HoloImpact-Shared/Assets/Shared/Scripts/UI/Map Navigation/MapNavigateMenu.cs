using System;
using UnityEngine;

/// <summary>
/// Synchronizes state between Map Navigation menu and the underlying Map Navigation FSM.
/// </summary>
public class MapNavigateMenu : MonoBehaviour
{
    public MapNavigationFSM mapNavigationFSM;
    public MapNavigationToggleGroup toggleGroup;

    protected virtual void Start()
    {
        if (!toggleGroup ||
            !mapNavigationFSM)
        {
            enabled = false;
            return;
        }

        foreach (MapNavigationState state in Enum.GetValues(typeof(MapNavigationState)))
        {
            Action modeChanged = () => OnFSMChanged(state);
            mapNavigationFSM.AddOnEnterListener(modeChanged, state);
        }

        toggleGroup.onValueChangedDelegate += OnToggleGroupChanged;
        toggleGroup.SetSelectedOption(mapNavigationFSM.GetCurrentState());
    }

    public void OnFSMChanged(MapNavigationState state)
    {
        toggleGroup.SetSelectedOption(state);
    }

    public void OnToggleGroupChanged(MapNavigationState state)
    {
        mapNavigationFSM.OnTransition(state);
    }
}