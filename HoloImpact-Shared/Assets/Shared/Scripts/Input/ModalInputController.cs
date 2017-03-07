using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An input controller that corresponds to a finite state machine.
/// <c>TState</c> correspond to the states.
/// <c>TTransition</c> corresponds to the transitions.
/// Ensures that only one registered input behaviour is active at a given time.
/// </summary>
[RequireComponent(typeof(BaseFiniteStateMachine))]
public abstract class ModalInputController<TState, TTransition> : BaseInputController
    where TState : struct, IConvertible
    where TTransition : struct, IConvertible
{
    private FiniteStateMachine<TState, TTransition> m_finiteStateMachine;
    private IDictionary<TState, Action> m_enterActions = new Dictionary<TState, Action>();
            IDictionary<TState, Action> m_exitActions = new Dictionary<TState, Action>();

    protected abstract IEnumerable<KeyValuePair<TState, Action>> GetEnterActions();
    protected abstract IEnumerable<KeyValuePair<TState, Action>> GetExitActions();

    protected override void Awake()
    {
        base.Awake();

        m_finiteStateMachine = GetComponent<BaseFiniteStateMachine>() as FiniteStateMachine<TState, TTransition>;

        foreach (var kvpair in GetEnterActions())
        {
            m_enterActions[kvpair.Key] = kvpair.Value;
            m_finiteStateMachine.AddOnEnterListener(kvpair.Value, kvpair.Key);
        }

        foreach (var kvpair in GetExitActions())
        {
            m_exitActions[kvpair.Key] = kvpair.Value;
            m_finiteStateMachine.AddOnExitListener(kvpair.Value, kvpair.Key);
        }
    }

    public override bool AddInput(RegisteredInputs name, IInputBehaviour behaviour)
    {
        if (base.AddInput(name, behaviour))
        {
            m_enterActions[m_finiteStateMachine.GetCurrentState()]();
            return true;
        }

        return false;
    }
}