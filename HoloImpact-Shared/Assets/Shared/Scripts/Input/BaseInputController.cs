using System.Collections.Generic;
using UnityEngine;

public interface IGazeInputController
{
    void OnFocusEnter();
    void OnFocusExit();
}

/// <summary>
/// Stores and maintains a list of current, valid input behaviours 
/// attached to an object. 
/// </summary>
public abstract class BaseInputController : MonoBehaviour
{
    private IDictionary<RegisteredInputs, IInputBehaviour> m_registeredInputs =
        new Dictionary<RegisteredInputs, IInputBehaviour>();

    protected abstract IEnumerable<RegisteredInputs> GetValidInputs();

    protected virtual void Awake()
    {
        foreach (var input in GetValidInputs())
        {
            m_registeredInputs[input] = null;
        }
    }

    public virtual bool AddInput(RegisteredInputs name, IInputBehaviour behaviour)
    {
        if (m_registeredInputs.ContainsKey(name))
        {
            m_registeredInputs[name] = behaviour;
            behaviour.Enable(false);
            return true;
        }

        return false;
    }

    public virtual IInputBehaviour GetInput(RegisteredInputs name)
    {
        IInputBehaviour retValue;
        m_registeredInputs.TryGetValue(name, out retValue);
        return retValue;
    }

    public virtual bool RemoveInput(RegisteredInputs name)
    {
        if (m_registeredInputs.ContainsKey(name))
        {
            m_registeredInputs[name] = null;
            return true;
        }

        return false;
    }
}