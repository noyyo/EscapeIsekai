using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNothingState : PlayerBaseState
{
    public PlayerNothingState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("dd");
        isMovable = false;
        StartAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
    }

    public override void Exit() 
    { 
        base.Exit();
        isMovable = true;
        StopAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
    }

}
