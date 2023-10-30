using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollState : PlayerGroundState
{

    public PlayerRollState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.MovementSpeedModifier = 0;
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.RollParameterHash);
        TryApplyForce();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.RollParameterHash);
    }


    private void TryApplyForce()
    {
        // 받고있던 힘을(Force) 리셋하고
        stateMachine.Player.ForceReceiver.Reset();
        // ForceReceiver에 AddForce를 적용. 바라보고있는 정면에서 밀려나도록.
        stateMachine.Player.ForceReceiver.AddForce(stateMachine.Player.transform.forward * groundData.RollForce);
    }


}
