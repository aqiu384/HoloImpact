using HoloToolkit.Unity;

/// <summary>
/// Rotates target to face user at all times.
/// </summary>
public class MyBillboard : Billboard, IInputBehaviour
{
    public void CopySettings(IInputSettings inputSettings)
    {
        var boardSettings = inputSettings as BillboardSettings;
        if (boardSettings != null)
        {
            switch(boardSettings.pivotAxis)
            {
                case BillboardSettings.PivotAxis.Free: PivotAxis = PivotAxis.Free; break;
                case BillboardSettings.PivotAxis.Y: PivotAxis = PivotAxis.Y; break;
            }
        }
    }

    public void Enable(bool shouldEnable)
    {
        enabled = shouldEnable;
    }

    public bool IsEnabled()
    {
        return enabled;
    }

    public void SetInputController(BaseInputController inputController) { }
}