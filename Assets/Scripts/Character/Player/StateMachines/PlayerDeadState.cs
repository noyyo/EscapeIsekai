using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.DeadParameterHash);
        DisablePlayerCollider();
    }
    public override void Exit()
    {
        base.Exit();
    }

    private void DisablePlayerCollider()
    {
        CapsuleCollider playercollider = stateMachine.Player.GetComponent<CapsuleCollider>();
        CharacterController characterController = stateMachine.Player.GetComponent<CharacterController>();

        if(playercollider != null && characterController != null)
        {
            playercollider.enabled = false;
            characterController.enabled = false;
        }
    }

    public override void Update()
    {
        base.Update();
    }
}
