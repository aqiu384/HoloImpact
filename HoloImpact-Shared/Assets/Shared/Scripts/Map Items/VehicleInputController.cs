using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens for valid input behavious on a vehicle object.
/// </summary>
[RequireComponent(typeof(Vehicle))]
public class VehicleInputController : BaseInputController, IGazeInputController
{
    [SerializeField]
    private Vehicle m_vehicle;
    [SerializeField]
    private VehicleSelectorController m_selectorController;

    protected virtual void Start()
    {
        m_vehicle = GetComponent<Vehicle>();
        GetComponent<MyNetworkTransform>().onParentPathChangeDelegate += OnEnable;

        //var mainSettings = m_particleSystem.main;
        //mainSettings.customSimulationSpace = transform.parent;
        //mainSettings.simulationSpace = ParticleSystemSimulationSpace.Custom;

        OnEnable();
    }

    protected virtual void OnEnable()
    {
        m_selectorController = GetComponentInParent<VehicleSelectorController>();
        if (m_selectorController) m_selectorController.AddItem(m_vehicle);
    }

    protected virtual void OnDisable()
    {
        if (m_selectorController) m_selectorController.DeleteItem(m_vehicle);
    }

    protected override IEnumerable<RegisteredInputs> GetValidInputs()
    {
        return new RegisteredInputs[]
        {
            RegisteredInputs.Hitbox
        };
    }

    public void OnFocusEnter()
    {
        m_selectorController.SelectItem(m_vehicle);
    }

    public void OnFocusExit() { }
}