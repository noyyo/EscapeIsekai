using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSuperJump : PlayerAirState
{
    public PlayerSuperJump(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {

        base.Enter();

        stateMachine.JumpForce = stateMachine.Player.Data.AirData.JumpForce;
        stateMachine.Player.ForceReceiver.Jump(10);
        StartAnimation(stateMachine.Player.AnimationData.SuperJumpParameterHash);
        stateMachine.Player.Playerconditions.UseSuperJump(groundData.SuperJumpCost);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.SuperJumpParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (stateMachine.Player.Controller.velocity.y <= 0)
        {
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }
    }
}
