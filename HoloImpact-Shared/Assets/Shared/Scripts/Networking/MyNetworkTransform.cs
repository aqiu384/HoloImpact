using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Ensures networked objects preserve their hiearchical location.
/// </summary>
[RequireComponent(typeof(NetworkIdentity))]
public class MyNetworkTransform : NetworkBehaviour
{
    public delegate void OnParentPathChangeDelegate();
    public OnParentPathChangeDelegate onParentPathChangeDelegate;

    [SerializeField]
    [SyncVar]
    private bool m_worldPositionStays = false;
    [SerializeField]
    [SyncVar(hook = "OnParentPathChange")]
    private string m_parentPath;

    public override void OnStartServer()
    {
        m_parentPath = Utilities.GetTransformPath(transform.parent);
    }

    public override void OnStartClient()
    {
        OnParentPathChange(m_parentPath);
    }

    private void OnParentPathChange(string parentPath)
    {
        var parent = GameObject.Find(parentPath);
        var parentTransform = parent ? parent.transform : null;

        transform.SetParent(parentTransform, m_worldPositionStays);
        if (onParentPathChangeDelegate != null) onParentPathChangeDelegate();
    }

    [Server]
    public void SetParent(Transform parentTransform, bool worldPositionStays = true)
    {
        m_worldPositionStays = worldPositionStays;
        m_parentPath = Utilities.GetTransformPath(parentTransform);
    }
}