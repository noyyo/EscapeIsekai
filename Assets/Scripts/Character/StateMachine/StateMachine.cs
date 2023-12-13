using System;
using UnityEngine;

[Serializable]
public abstract class StateMachine
{
    public IState CurrentState { get; protected set; }
    [SerializeField][ReadOnly] private string currentStateName;
    public bool IsInStateTransition;
    public void ChangeState(IState newState)
    {
        IsInStateTransition = true;
        CurrentState?.Exit();

        CurrentState = newState;

        CurrentState?.Enter();

        currentStateName = CurrentState.GetType().Name;
        IsInStateTransition = false;
    }

    public virtual void HandleInput()
    {
        CurrentState?.HandleInput();
    }

    public virtual void Update()
    {
        CurrentState?.Update();
    }

    public virtual void PhysicsUpdate()
    {
        CurrentState?.PhysicsUpdate();
    }
}