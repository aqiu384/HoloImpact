public enum NetworkManagerState
{
    Disconnected,
    ClientConnecting,
    HostConnecting,
    ClientConnected,
    HostConnected
}

public enum NetworkManagerTransition
{
    ConnectHost,
    ConnectClient,
    CancelConnection,
    ServerResponded
}

public class NetworkManagerFSM : FiniteStateMachine<NetworkManagerState, NetworkManagerTransition> { }