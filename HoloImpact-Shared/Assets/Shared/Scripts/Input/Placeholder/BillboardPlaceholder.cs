using UnityEngine;
using System;

[Serializable]
public class BillboardSettings : IInputSettings
{
    public enum PivotAxis
    {
        // Rotate about all axes.
        Free,
        // Rotate about an individual axis.
        Y
    }

    [Tooltip("Specifies the axis about which the object will rotate.")]
    public PivotAxis pivotAxis = PivotAxis.Free;

    public RegisteredInputs GetInputType()
    {
        return RegisteredInputs.Billboard;
    }
}

public class BillboardPlaceholder : InputPlaceholder
{
    public BillboardSettings settings;

    public override IInputSettings GetInputSettings()
    {
        return settings;
    }
}