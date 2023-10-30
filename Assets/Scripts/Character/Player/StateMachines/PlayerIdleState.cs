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
        // Idle 상태는 움직임이 없어야 하기 때문에 이동에 대한 처리를 방지하도록 처리
        stateMachine.MovementSpeedModifier = 0f;
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
        // 이동이 일어난다면 OnMove를 실행한다. idle에서 키 입력이 되면 walk로 전환,
        // walk에서 키 입력이 되면 run이 되는 등
        // 이동처리가 되면 state에서 onmove로 전환을 시킨다
        // 즉, 키입력이 되면 각각의 스테이트로 전환을 시킨다.
        if(stateMachine.MovementInput != Vector2.zero)
        {
            OnMove();
            return;
        }
    }
}
