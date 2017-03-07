public enum VehicleSelectorState
{
    Default,
    JumpTo,
    Track,
    Info,
    Processing
}

public enum VehicleSelectorTransition
{
    Cancel,
    JumpTo,
    Track,
    Info,
    VehicleSelected,
    FinishedProcessing
}

public class VehicleSelectorFSM : FiniteStateMachine<VehicleSelectorState, VehicleSelectorTransition> { }