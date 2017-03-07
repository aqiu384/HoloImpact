using System;
using System.Collections.Generic;

/// <summary>
/// Converts a placeholder input behaviour to the correct behaviour
/// for this platform.
/// </summary>
public abstract class InputConverter : MySingleton<InputConverter>
{
    private IDictionary<Type, Type> m_inputConversions;
    protected abstract IDictionary<Type, Type> GetInputConversions();

    protected override void Awake()
    {
        base.Awake();
        m_inputConversions = GetInputConversions();
    }

    public Type ConvertInput(InputPlaceholder inputSettings)
    {
        Type outType;
        m_inputConversions.TryGetValue(inputSettings.GetType(), out outType);
        return outType;
    }
}