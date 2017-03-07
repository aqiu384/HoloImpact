using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use <c>GetComponent</c> on this to get <c>FiniteStateMachine</c> without
/// needing generics.
/// </summary>
public abstract class BaseFiniteStateMachine : MonoBehaviour { }

/// <summary>
/// Wrapper around Unity's Animator Finite State Machine.
/// <c>TState</c> enumerates the states in the Animator.
/// <c>TTransition</c> enumerates the triggers in the Animator.
/// Accepts transitions and sets the corresponding triggers in the FSM
/// and notifies all listeners when a state is entered or exited.
/// </summary>
[RequireComponent(typeof(Animator))]
public abstract class FiniteStateMachine<TState, TTransition> : BaseFiniteStateMachine
    where TState : struct, IConvertible
    where TTransition : struct, IConvertible
{
    public static readonly int NUM_STATES = Enum.GetNames(typeof(TState)).Length;
    private static IDictionary<int, TState> m_stateIDs;

    private Animator m_animator;
    private Action[] m_onEnterStateActions = new Action[NUM_STATES],
                     m_onExitStateActions = new Action[NUM_STATES];

    protected virtual void Awake()
    {
        if (m_stateIDs == null)
        {
            m_stateIDs = new Dictionary<int, TState>();
            foreach (TState state in Enum.GetValues(typeof(TState)))
            {
                m_stateIDs[Animator.StringToHash(state.ToString())] = state;
            }
        }

        m_animator = GetComponent<Animator>();
    }

    public void AddOnEnterListener(Action action, TState state)
    {
        m_onEnterStateActions[Convert.ToInt32(state)] += action;
    }

    public void AddOnExitListener(Action action, TState state)
    {
        m_onExitStateActions[Convert.ToInt32(state)] += action;
    }

    public void RemoveOnEnterListener(Action action, TState state)
    {
        m_onEnterStateActions[Convert.ToInt32(state)] -= action;
    }

    public void RemoveOnExitListener(Action action, TState state)
    {
        m_onExitStateActions[Convert.ToInt32(state)] -= action;
    }

    /// <summary>
    /// Notify all listeners when a state is entered.
    /// <param name="shortNameHash">Unity's hash value for a state</param>
    /// </summary>
    public void OnStateEnter(int shortNameHash)
    {
        OnStateEnter(m_stateIDs[shortNameHash]);
    }

    public void OnStateEnter(TState state)
    {
        var enterActions = m_onEnterStateActions[Convert.ToInt32(state)];
        if (enterActions != null) enterActions();
    }

    /// <summary>
    /// Notify all listeners when a state is exited.
    /// <param name="shortNameHash">Unity's hash value for a state</param>
    /// </summary>
    public void OnStateExit(int shortNameHash)
    {
        OnStateExit(m_stateIDs[shortNameHash]);
    }

    public void OnStateExit(TState state)
    {
        var exitActions = m_onExitStateActions[Convert.ToInt32(state)];
        if (exitActions != null) exitActions();
    }

    public void OnTransition(TTransition transition)
    {
        m_animator.SetTrigger(transition.ToString());
    }

    public TState GetCurrentState()
    {
        return m_stateIDs[m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash];
    }
}