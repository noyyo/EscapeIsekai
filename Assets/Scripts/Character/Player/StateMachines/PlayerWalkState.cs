using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerGroundState
{
    public PlayerWalkState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log(stateMachine.MovementSpeedModifier);
        base.Enter();
        stateMachine.MovementSpeed = groundData.WalkSpeed;
        StartAnimation(stateMachine.Player.AnimationData.WalkParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.WalkParameterHash);
    }

    protected override void OnRunStarted(InputAction.CallbackContext context)
    {
        base.OnRunStarted(context);
        stateMachine.ChangeState(stateMachine.RunState);
    }
}
