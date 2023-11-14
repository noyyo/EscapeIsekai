using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
    }

    public override void Update()
    {
        base.Update();
        // �̵��� �Ͼ�ٸ� OnMove�� �����Ѵ�. idle���� Ű �Է��� �Ǹ� walk�� ��ȯ,
        // walk���� Ű �Է��� �Ǹ� run�� �Ǵ� ��
        // �̵�ó���� �Ǹ� state���� onmove�� ��ȯ�� ��Ų��
        // ��, Ű�Է��� �Ǹ� ������ ������Ʈ�� ��ȯ�� ��Ų��.
        if(stateMachine.MovementInput != Vector2.zero)
        {
            OnMove();
            return;
        }
    }
}
