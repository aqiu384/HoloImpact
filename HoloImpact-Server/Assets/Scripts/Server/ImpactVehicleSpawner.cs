using System;
using System.Collections.Generic;
using UnityEngine;
using Afrl.Lmcp;
using Afrl.Cmasi;
using Afrl.Impact;

/// <summary>
/// Spawns and updates vehicles according to messages received
/// on the IMPACT Hub subscriber.
/// </summary>  
public class ImpactVehicleSpawner : VehicleSpawner
{
    private IDictionary<Type, VehicleType> m_vehicleTypes;
    private ImpactHubSubscriber m_hubSubscriber;

    protected virtual void Awake()
    {
        m_hubSubscriber = GetComponent<ImpactHubSubscriber>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        m_vehicleTypes = new Dictionary<Type, VehicleType>()
        {
            { typeof(GroundVehicleState), VehicleType.Ground },
            { typeof(SurfaceVehicleState), VehicleType.Surface },
            { typeof(AirVehicleState), VehicleType.Air }
        };

        m_hubSubscriber.onImpactHubSubscriberStartedDelegate += StartSpawner;
        m_hubSubscriber.onImpactHubSubscriberStoppedDelegate += StopSpawner;
        m_hubSubscriber.onImpactMessageReceivedDelegate += ProcessImpactObject;
    }

    public void ProcessImpactObject(ILmcpObject impactObject)
    {
        var entityState = impactObject as EntityState;
        var location = entityState.Location;

        var vehicleState = new VehicleState();
        vehicleState.id = entityState.ID;
        vehicleState.vehicleType = m_vehicleTypes[entityState.GetType()];
        vehicleState.timestamp = entityState.Time;
        vehicleState.track.location = new Vector3((float)location.Longitude, location.Altitude / 4000.0f, (float)location.Latitude);
        vehicleState.track.velocity = new Vector3(entityState.U, entityState.V, entityState.W) / 1000.0f;

        ProcessVehicleState(vehicleState);
    }
}