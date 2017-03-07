using UnityEngine;
using UnityEngine.Networking;

public enum VehicleType
{
    Ground,
    Surface,
    Air
}

public struct TrackRecord
{
    public Vector3 location;
    public Vector3 velocity;
    public Vector3 acceleration;
}

public struct VehicleState
{
    public long id;
    public VehicleType vehicleType;
    public long timestamp;
    public TrackRecord track;
}

/// <summary>
/// Represents an IMPACT vehicle.
/// </summary>
[RequireComponent(typeof(NetworkTransform))]
public class Vehicle : NetworkBehaviour, IMapItem
{
    [SyncVar(hook = "OnCenterChange")]
    public Vector3 center;
    [SyncVar(hook = "OnVehicleStateChange")]
    public VehicleState vehicleState;

    public delegate void OnTrackChangeDelegate(TrackRecord track);
    public OnTrackChangeDelegate onTrackChangeDelegate;
    private Vector3 m_velocity;

    protected virtual void Update()
    {
        var deltaTime = Time.deltaTime;

        m_velocity += vehicleState.track.acceleration * deltaTime;
        transform.localPosition += m_velocity * deltaTime;
    }

    private void UpdateLocalPosition(Vector3 newLocation, Vector3 newCenter)
    {
        transform.localPosition = newLocation - newCenter;
    }

    private void OnCenterChange(Vector3 newCenter)
    {
        UpdateLocalPosition(vehicleState.track.location, newCenter);
    }

    private void OnVehicleStateChange(VehicleState newState)
    {
        var heading = newState.track.velocity;
        if (heading != Vector3.zero)
        {
            transform.forward = heading;
        }

        m_velocity = newState.track.velocity;
        UpdateLocalPosition(newState.track.location, center);

        if (onTrackChangeDelegate != null) onTrackChangeDelegate(newState.track);
    }

    public long GetID()
    {
        return vehicleState.id;
    }
}