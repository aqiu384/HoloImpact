public enum MapNavigationState
{
    Place,
    Drag,
    Zoom,
    Scale
}

public class MapNavigationFSM : FiniteStateMachine<MapNavigationState, MapNavigationState> { }