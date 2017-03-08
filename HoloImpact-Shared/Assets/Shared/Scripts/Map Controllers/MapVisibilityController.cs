using UnityEngine;

/// <summary>
/// Notifies objects whenever they cross the map window boundary.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TransformOverTime))]
public class MapVisibilityController : MonoBehaviour
{
    private TransformOverTime m_timeTransform;

    public NetworkManagerFSM networkManagerFSM;
    public GameObject mainMap;
    public Vector3 initialMapPosition = new Vector3(0, 0.45f, 0);
    public Vector3 initialMapScale = new Vector3(2, 0.5f, 2);
    public float mapReloadAnimationTime = 1.0f;

    protected virtual void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        m_timeTransform = GetComponent<TransformOverTime>();

        if (networkManagerFSM)
        {
            networkManagerFSM.AddOnEnterListener(PlayMapReloadAnimation, NetworkManagerState.HostConnected);
            networkManagerFSM.AddOnEnterListener(PlayMapReloadAnimation, NetworkManagerState.ClientConnected);
            networkManagerFSM.AddOnExitListener(PlayMapReloadAnimation, NetworkManagerState.HostConnected);
            networkManagerFSM.AddOnExitListener(PlayMapReloadAnimation, NetworkManagerState.ClientConnected);
        }

        transform.localPosition = initialMapPosition;
        transform.localScale = initialMapScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        var mapVisibility = other.GetComponent<MapVisibility>();
        if (mapVisibility) mapVisibility.OnEnterMap();
    }

    private void OnTriggerExit(Collider other)
    {
        var mapVisibility = other.GetComponent<MapVisibility>();
        if (mapVisibility) mapVisibility.OnExitMap();
    }

    public void PlayMapReloadAnimation()
    {
        m_timeTransform.TransitionTowardsLocal(initialMapPosition, Vector3.up * initialMapScale.y, 2 / mapReloadAnimationTime);
        Invoke("GrowMapToFullSize", mapReloadAnimationTime / 2);
    }

    private void GrowMapToFullSize()
    {
        m_timeTransform.TransitionTowardsLocal(initialMapPosition, initialMapScale, 2 / mapReloadAnimationTime);
    }
}