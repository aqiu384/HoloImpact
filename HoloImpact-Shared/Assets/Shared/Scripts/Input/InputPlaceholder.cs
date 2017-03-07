using UnityEngine;

public interface IInputSettings
{
    RegisteredInputs GetInputType();
}

public interface IInputBehaviour
{
    void CopySettings(IInputSettings inputSettings);
    void Enable(bool shouldEnable);
    bool IsEnabled();
    void SetInputController(BaseInputController inputController);
}

/// <summary>
/// A placeholder behaviour for platform-dependent input controls.
/// Hold a list of settings to be copied over.
/// </summary>
public abstract class InputPlaceholder : MonoBehaviour
{
    public abstract IInputSettings GetInputSettings();

    protected virtual void Start()
    {
        var inputConverter = InputConverter.Instance;
        if (inputConverter)
        {
            var inputBehaviour = gameObject.AddComponent(inputConverter.ConvertInput(this)) as IInputBehaviour;
            if (inputBehaviour != null)
            {
                var settings = GetInputSettings();
                inputBehaviour.CopySettings(settings);

                var inputController = GetComponentInParent<BaseInputController>();
                if (inputController && inputController.AddInput(settings.GetInputType(), inputBehaviour))
                {
                    inputBehaviour.SetInputController(inputController);
                }
            }
        }

        Destroy(this);
    }
}