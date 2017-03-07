using UnityEngine;

/// <summary>
/// Transforms an object to a target gradually over time.
/// </summary>
public class TransformOverTime : MonoBehaviour
{
    private Vector3 m_startPosition, m_targetPosition;
    private Quaternion m_startRotation, m_targetRotation;
    private Vector3 m_startScale, m_targetScale;
    private float m_startTime, m_targetDuration;
    private bool m_isTransforming;

    protected virtual void Update()
    {
        if (m_isTransforming)
        {
            var interpolant = (Time.time - m_startTime) / m_targetDuration;

            if (interpolant >= 1.0f)
            {
                m_isTransforming = false;
            }

            transform.localPosition = Vector3.Lerp(m_startPosition, m_targetPosition, interpolant);
            transform.localRotation = Quaternion.Lerp(m_startRotation, m_targetRotation, interpolant);
            transform.localScale = Vector3.Lerp(m_startScale, m_targetScale, interpolant);
        }
    }

    private void TransitionTowardsLocal()
    {
        m_startPosition = transform.localPosition;
        m_startRotation = transform.localRotation;
        m_startScale = transform.localScale;

        m_startTime = Time.time;
        m_isTransforming = true;
    }

    public void TransitionTowardsLocal(Vector3 localPosition, float speed)
    {
        m_targetPosition = localPosition;
        m_targetRotation = transform.localRotation;
        m_targetScale = transform.localScale;
        m_targetDuration = 1 / speed;

        TransitionTowardsLocal();
    }

    public void TransitionTowardsLocal(Vector3 localPosition, Vector3 localScale, float speed)
    {
        m_targetPosition = localPosition;
        m_targetRotation = transform.localRotation;
        m_targetScale = localScale;
        m_targetDuration = 1 / speed;

        TransitionTowardsLocal();
    }

    public void TransitionTowardsLocal(Vector3 localPosition, Quaternion localRotation, Vector3 localScale, float speed)
    {
        m_targetPosition = localPosition;
        m_targetRotation = localRotation;
        m_targetScale = localScale;
        m_targetDuration = 1 / speed;

        TransitionTowardsLocal();
    }
}