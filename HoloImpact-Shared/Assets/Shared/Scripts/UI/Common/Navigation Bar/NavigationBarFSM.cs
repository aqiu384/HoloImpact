public enum NavigationBarState
{
    Hidden,
    Minimizing,
    Maximizing,
    Showing,
    Exiting,
    Dragging
}

public enum NavigationBarTransition
{
    Minimize,
    Maximize,
    Exit,
    StartDragging,
    StopDragging
}

public class NavigationBarFSM : FiniteStateMachine<NavigationBarState, NavigationBarTransition> { }