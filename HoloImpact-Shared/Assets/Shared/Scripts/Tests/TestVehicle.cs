using UnityEngine;
using UnityEngine.Networking;

public class TestVehicle : MonoBehaviour
{
    // San Diego
    public long id = 1;
    public VehicleType vehicleType = VehicleType.Ground;

    public Vector3 orbitCenter = new Vector3(-117.1611f, 0, 32.7157f);
    public float latDistance = 1.0f, longDistance = 1.0f;
    public float orbitPeriod = 1.0f, updateFrequency = 1.0f;

    private TrackRecord m_track;
    private float m_angularVelocity;
    private Vector3 m_velocity;

    protected virtual void Start()
    {
        m_angularVelocity = 2 * Mathf.PI / orbitPeriod;
        InvokeRepeating("SendTrackRecord", 1 / updateFrequency, 1 / updateFrequency);
    }

    private void SendTrackRecord()
    {
        var currentTime = Time.time;
        var orbitAngle = m_angularVelocity * currentTime;
        TrackRecord track;

        track.location = new Vector3(
            longDistance * Mathf.Cos(orbitAngle),
            0,
            latDistance * Mathf.Sin(orbitAngle)
        ) + orbitCenter;

        track.velocity = new Vector3(
            longDistance * -1 * m_angularVelocity * Mathf.Sin(orbitAngle),
            0,
            latDistance * m_angularVelocity * Mathf.Cos(orbitAngle)
        );

        track.acceleration = new Vector3(
            longDistance * -1 * m_angularVelocity * m_angularVelocity * Mathf.Cos(orbitAngle),
            0,
            latDistance * -1 * m_angularVelocity * m_angularVelocity * Mathf.Sin(orbitAngle)
        );

        OnVehicleStateChange(track);
    }

    protected virtual void Update()
    {
        var deltaTime = Time.deltaTime;

        m_velocity += m_track.acceleration * deltaTime;
        transform.localPosition += m_velocity * deltaTime;

        Debug.Log(transform.localPosition);
    }

    private void OnVehicleStateChange(TrackRecord track)
    {
        m_track = track;
        transform.localPosition = track.location - orbitCenter;
        m_velocity = m_track.velocity;
    }
}