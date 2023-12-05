using System;
using UnityEngine;

[Serializable]
public abstract class StateMachine
{
    public IState currentState { get; protected set; }
    [SerializeField][ReadOnly] private string currentStateName;
    public void ChangeState(IState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState?.Enter();

        currentStateName = currentState.GetType().Name;
    }

    public virtual void HandleInput()
    {
        currentState?.HandleInput();
    }

    public virtual void Update()
    {
        currentState?.Update();
    }

    public virtual void PhysicsUpdate()
    {
        currentState?.PhysicsUpdate();
    }
}