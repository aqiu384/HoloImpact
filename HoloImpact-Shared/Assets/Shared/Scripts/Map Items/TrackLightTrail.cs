using System;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class TrackLightTrail : MonoBehaviour
{
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public float averageUpdateTime = 1.0f;

    public int numPredictedSteps = 1;
    public int numSavedTracks = 5;
    private int m_trailLength;

    // private int m_predictedStepIndex;
    private Vehicle m_vehicle;
    private Transform m_lightTrailTransform;
    private LineRenderer m_lineRenderer;
    private Vector3[] m_currentPoints;
    private float m_lastUpdateTime;
    private Vector3 m_trailEndpoint;

    protected virtual void Start()
    {
        m_trailLength = numPredictedSteps + numSavedTracks;

        m_vehicle = transform.gameObject.GetComponent<Vehicle>();
        m_vehicle.onTrackChangeDelegate += OnTrackChange;

        m_currentPoints = new Vector3[m_trailLength];

        m_lightTrailTransform = new GameObject(string.Format("{0} Light Trail", name)).transform;
        m_lightTrailTransform.parent = transform.parent;
        m_lightTrailTransform.localPosition = Vector3.zero;
        m_lightTrailTransform.gameObject.layer = Constants.MAP_TERRAIN_LAYER;

        m_lineRenderer = m_lightTrailTransform.gameObject.AddComponent<LineRenderer>();
        m_lineRenderer.startWidth = 0.1f;
        m_lineRenderer.endWidth = 0.1f;
        m_lineRenderer.numCapVertices = 5;
        m_lineRenderer.useWorldSpace = false;
        m_lineRenderer.widthMultiplier = 0.2f;
        m_lineRenderer.numPositions = m_trailLength;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        m_lineRenderer.colorGradient = gradient;

        OnTrackChange(m_vehicle.vehicleState.track);
        m_lineRenderer.SetPositions(m_currentPoints);
    }

    protected virtual void OnDestroy()
    {
        Destroy(m_lightTrailTransform.gameObject);
    }

    protected virtual void Update()
    {
        // Extend head
        // Array.Copy(m_currentPoints, 0, m_currentPoints, 1, numPredictedSteps - 1);
        m_currentPoints[0] = m_lightTrailTransform.InverseTransformPoint(transform.position);

        // Shorten tail
        var interpolant = (Time.time - m_lastUpdateTime) / averageUpdateTime;
        if (interpolant < 1.0f)
        {
            m_currentPoints[m_trailLength - 1] = Vector3.Lerp(
                m_trailEndpoint,
                m_currentPoints[m_trailLength - 2],
                interpolant
            );
        }

        m_lineRenderer.SetPositions(m_currentPoints);
    }

    public void OnTrackChange(TrackRecord track)
    {
        Array.Copy(m_currentPoints, numPredictedSteps, m_currentPoints, numPredictedSteps + 1, numSavedTracks - 1);
        m_currentPoints[numPredictedSteps].y = m_currentPoints[0].y;

        var updatedLocation = track.location - m_vehicle.center;
        for (var i = 0; i <= numPredictedSteps; i++)
        {
            m_currentPoints[i] = updatedLocation;
        }

        m_trailEndpoint = m_currentPoints[m_trailLength - 1];
        m_lastUpdateTime = Time.time;
        // m_predictedStepIndex = numPredictedSteps - 1;
    }
}