public enum TestCubeState
{
    Minimized,
    Growing,
    Maximized,
    Shrinking
}

public enum TestCubeTransition
{
    StartGrowing,
    FinishedGrowing,
    StartShrinking,
    FinishedShrinking
}

public class TestCubeFSM : FiniteStateMachine<TestCubeState, TestCubeTransition> { }