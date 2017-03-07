using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Network manager that also synchronizes with underling Network State FSM.
/// </summary>
[RequireComponent(typeof(NetworkManagerFSM))]
public class MyNetworkManager : NetworkManager
{
    [SerializeField]
    private NetworkManagerFSM m_networkManagerFSM;
    public static MyNetworkManager Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        m_networkManagerFSM = GetComponent<NetworkManagerFSM>();

        m_networkManagerFSM.AddOnEnterListener(StartClientAsAction, NetworkManagerState.ClientConnecting);
        m_networkManagerFSM.AddOnEnterListener(StartHostAsAction, NetworkManagerState.HostConnecting);
        m_networkManagerFSM.AddOnEnterListener(StopHost, NetworkManagerState.Disconnected);
    }

    public void StartClientAsAction()
    {
        StartClient();
    }

    public void StartHostAsAction()
    {
        if (StartHost() == null)
        {
            m_networkManagerFSM.OnTransition(NetworkManagerTransition.CancelConnection);
        };
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        m_networkManagerFSM.OnTransition(NetworkManagerTransition.ServerResponded);
        base.OnClientConnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        m_networkManagerFSM.OnTransition(NetworkManagerTransition.CancelConnection);
        base.OnClientDisconnect(conn);
    }
}