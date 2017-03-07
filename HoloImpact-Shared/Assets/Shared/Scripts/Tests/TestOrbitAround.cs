using UnityEngine;

public class TestOrbitAround : MonoBehaviour
{
    public float orbitPeriod = 10.0f;

    private float m_localDistance;
    private float m_angularVelocity;

    protected virtual void Awake()
    {
        m_localDistance = transform.localPosition.magnitude;
        m_angularVelocity = 2 * Mathf.PI / orbitPeriod;
    }

    protected virtual void Update()
    {
        var currentTime = Time.time;

        transform.localPosition = m_localDistance * new Vector3(
            Mathf.Cos(currentTime * m_angularVelocity),
            0,
            Mathf.Sin(currentTime * m_angularVelocity)
        );
    }
}