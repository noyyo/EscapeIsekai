using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �߻� Ŭ������ �����Ͽ� ����� �޾Ƽ� ����� �� �ֵ��� ��.
// StateMachine�� ��ü������ ��üȭ�� �� �� ����
public abstract class StateMachine
{
    protected IState currentState;  // ���� ���¸� �� �� �ְ� ����

    public void ChangeState(IState newState)
    {
        currentState?.Exit();   // ���� ���¿��� �������ͼ�
        currentState = newState;    // ���� ���¸� �޾Ƽ� ���ο� ���·� �ٲ���
        currentState?.Enter();  // ���ο� ���·� �ٲ� ���� ���·� ���� ��
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
