using UnityEngine;
using UnityEngine.Networking;

public class VehicleTrackSimulator : NetworkBehaviour
{
    // San Diego
    public long id = 1;
    public VehicleType vehicleType = VehicleType.Ground;

    public Vector3 orbitCenter = new Vector3(-117.1611f, 0, 32.7157f);
    public float latDistance = 1.0f, longDistance = 1.0f;
    public float orbitPeriod = 1.0f, updateFrequency = 1.0f;

    private VehicleSpawner m_vehicleSpawner;
    private VehicleState m_vehicleState;
    private float m_angularVelocity;

    protected virtual void OnEnable()
    {
        m_vehicleSpawner = GetComponentInParent<VehicleSpawner>();
        m_vehicleState.id = id;
        m_vehicleState.vehicleType = vehicleType;
        m_angularVelocity = 2 * Mathf.PI / orbitPeriod;
        InvokeRepeating("SendTrackRecord", 1 / updateFrequency, 1 / updateFrequency);
    }

    protected virtual void OnDisable()
    {
        CancelInvoke();
    }

    private void SendTrackRecord()
    {
        var currentTime = Time.time;
        var orbitAngle = m_angularVelocity * currentTime;

        m_vehicleState.timestamp = (long)currentTime * 1000;

        m_vehicleState.track.location = new Vector3(
            longDistance * Mathf.Cos(orbitAngle), 
            0, 
            latDistance * Mathf.Sin(orbitAngle)
        ) + orbitCenter;

        m_vehicleState.track.velocity = new Vector3(
            longDistance * -1 * m_angularVelocity * Mathf.Sin(orbitAngle), 
            0, 
            latDistance * m_angularVelocity * Mathf.Cos(orbitAngle)
        );

        m_vehicleState.track.acceleration = new Vector3(
            longDistance * -1 * m_angularVelocity * m_angularVelocity * Mathf.Cos(orbitAngle),
            0,
            latDistance * -1 * m_angularVelocity * m_angularVelocity * Mathf.Sin(orbitAngle)
        );

        m_vehicleSpawner.ProcessVehicleState(m_vehicleState);
    }
}