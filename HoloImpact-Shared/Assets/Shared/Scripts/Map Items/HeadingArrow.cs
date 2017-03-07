using UnityEngine;

public class HeadingArrow : MonoBehaviour
{
    public Transform arrowHead, arrowBody;

    [SerializeField]
    private Vector3 m_headPosition;
    [SerializeField]
    private Vector3 m_tailPosition;

    [SerializeField]
    public Vector3 headPosition
    {
        get { return m_headPosition; }
        set { m_headPosition = value; UpdateArrowBounds(); }
    }

    [SerializeField]
    public Vector3 tailPosition
    {
        get { return m_tailPosition; }
        set { m_tailPosition = value; UpdateArrowBounds(); }
    }

    protected virtual void Awake()
    {
        m_headPosition = transform.position;
        m_tailPosition = transform.position;
    }

    private void UpdateArrowBounds()
    {
        var bodyScale = arrowBody.localScale;
        bodyScale.y = transform.InverseTransformVector(headPosition - tailPosition).magnitude / 2;

        arrowBody.position = (headPosition + tailPosition) / 2;
        arrowBody.up = headPosition - tailPosition;
        arrowBody.localScale = bodyScale;

        arrowHead.position = headPosition;
        arrowHead.forward = arrowBody.up;
    }
}