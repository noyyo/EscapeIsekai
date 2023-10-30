using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 추상 클래스로 선언하여 상속을 받아서 사용할 수 있도록 함.
// StateMachine은 자체적으로 객체화를 할 수 없음
public abstract class StateMachine
{
    protected IState currentState;  // 현재 상태를 알 수 있게 설정

    public void ChangeState(IState newState)
    {
        currentState?.Exit();   // 현재 상태에서 빠져나와서
        currentState = newState;    // 현재 상태를 받아서 새로운 상태로 바꿔줌
        currentState?.Enter();  // 새로운 상태로 바뀐 현재 상태로 들어가게 함
    }

    public void HandleInput()
    {
        currentState?.HandleInput();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void PhysicsUpdate()
    {
        currentState?.PhysicsUpdate();
    }
}
