using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine
{
    protected IState currentState;

    public void ChangeState(IState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState?.Enter();
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