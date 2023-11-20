using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowState : PlayerGroundState
{
    public PlayerThrowState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isMovable = false;
        StartAnimation(stateMachine.Player.AnimationData.ThrowParameterHash);
        stateMachine.Player.CreateGrenade();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.ThrowParameterHash);
    }

    public override void Update()
    {
        base.Update();
        if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Throw") &&
            stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    
}
