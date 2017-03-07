using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Synchronizes state between Network Connection menu and underlying Network Connection FSM.
/// </summary>
[AddComponentMenu("UI/MyNetworkManagerHUD")]
public class MyNetworkManagerHUD : MonoBehaviour
{
    public NetworkManagerFSM networkManagerFSM;
    public GameObject disconnectedSubmenu, clientConnectingSubmenu, clientSubmenu, hostSubmenu;

    private bool CheckPreconditions()
    {
        if (networkManagerFSM == null ||
            disconnectedSubmenu == null ||
            clientConnectingSubmenu == null ||
            clientSubmenu == null ||
            hostSubmenu == null)
        {
            return false;
        }

        var submenus = new Dictionary<NetworkManagerState, GameObject>()
        {
            { NetworkManagerState.Disconnected, disconnectedSubmenu },
            { NetworkManagerState.ClientConnecting, clientConnectingSubmenu },
            { NetworkManagerState.ClientConnected, clientSubmenu },
            { NetworkManagerState.HostConnected, hostSubmenu },
        };

        foreach (var kvpair in submenus)
        {
            var state = kvpair.Key;
            var submenu = kvpair.Value;

            networkManagerFSM.AddOnEnterListener(() => submenu.SetActive(true), state);
            networkManagerFSM.AddOnExitListener(() => submenu.SetActive(false), state);
            submenu.SetActive(false);
        }

        return true;
    }

    protected virtual void Start()
    {
        if (!CheckPreconditions())
        {
            Destroy(gameObject);
        }
    }

    public void StartHost()
    {
        networkManagerFSM.OnTransition(NetworkManagerTransition.ConnectHost);
    }

    public void StartClient()
    {
        networkManagerFSM.OnTransition(NetworkManagerTransition.ConnectClient);
    }

    public void StopConnection()
    {
        networkManagerFSM.OnTransition(NetworkManagerTransition.CancelConnection);
    }
}