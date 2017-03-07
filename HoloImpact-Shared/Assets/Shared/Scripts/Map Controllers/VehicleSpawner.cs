using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Base controller for spawning and updating vehicles.
/// </summary>
[RequireComponent(typeof(NetworkIdentity))]
public class VehicleSpawner : NetworkBehaviour
{
    public Transform spawnArea;
    public GameObject GVPrefab, UAVPrefab;
    public Vector3 mapCenter = new Vector3(-117.1611f, 0, 32.7157f);

    private IDictionary<VehicleType, GameObject> m_vehiclePrefabs;
    private IDictionary<long, Vehicle> m_vehicles = new Dictionary<long, Vehicle>();

    public override void OnStartServer()
    {
        m_vehiclePrefabs = new Dictionary<VehicleType, GameObject>()
        {
            { VehicleType.Ground, GVPrefab },
            { VehicleType.Surface, GVPrefab },
            { VehicleType.Air, UAVPrefab }
        };

        GetComponent<NetworkIdentity>().serverOnly = true;
    }

    public virtual void OnDisable()
    {
        StopSpawner();
    }

    public void StartSpawner(Vector3 newMapCenter)
    {
        mapCenter = newMapCenter;
    }

    public void StopSpawner()
    {
        m_vehicles.Clear();
    }

    [Server]
    public void ProcessVehicleState(VehicleState vehicleState)
    {
        try
        {
            var vehicle = m_vehicles[vehicleState.id];
            vehicle.vehicleState = vehicleState;
        }
        catch (KeyNotFoundException)
        {
            var vehicleObject = Instantiate(m_vehiclePrefabs[vehicleState.vehicleType]);
            var vehicle = vehicleObject.GetComponent<Vehicle>();

            vehicle.vehicleState = vehicleState;
            vehicle.center = mapCenter;

            NetworkServer.Spawn(vehicleObject);

            vehicle.GetComponent<MyNetworkTransform>().SetParent(spawnArea);
            m_vehicles[vehicleState.id] = vehicle;
        }
    }
}