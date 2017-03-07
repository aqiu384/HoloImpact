using System;
using UnityEngine;

/// <summary>
/// Attach this to all states in an Unity's Animator Finite State Machine.
/// <c>TState</c> enumerates the states in the Animator.
/// <c>TTransition</c> enumerates the triggers in the Animator.
/// Enables this state to notify wrapper FSM upon enter and exit.
/// </summary>
public abstract class FSMState<TState, TTransition> : StateMachineBehaviour
    where TState : struct, IConvertible
    where TTransition : struct, IConvertible
{
    private FiniteStateMachine<TState, TTransition> GetParentFSM(Animator animator)
    {
        return animator.gameObject.GetComponent<BaseFiniteStateMachine>() as FiniteStateMachine<TState, TTransition>;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var parentFSM = GetParentFSM(animator);
        parentFSM.OnStateEnter(stateInfo.shortNameHash);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var parentFSM = GetParentFSM(animator);
        parentFSM.OnStateExit(stateInfo.shortNameHash);
    }
}