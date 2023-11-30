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
        base.Enter();
        isMovable = true;
        isStateChangeable = false;
        StartAnimation(stateMachine.Player.AnimationData.RollParameterHash);
        TryApplyForce();
        stateMachine.Player.Playerconditions.UseStamina(groundData.StaminaCost);
        DisablePlayerCollider();
    }

    public override void Exit()
    {
        base.Exit();
        isMovable = true;
        isStateChangeable = true;
        StopAnimation(stateMachine.Player.AnimationData.RollParameterHash);
        EnablePlayerCollider();
    }


    private void TryApplyForce()
    {
        stateMachine.Player.ForceReceiver.Reset();
        stateMachine.Player.ForceReceiver.AddForce(stateMachine.Player.transform.forward * groundData.RollForce);
    }

    private void DisablePlayerCollider()
    {
        CapsuleCollider playercollider = stateMachine.Player.GetComponent<CapsuleCollider>();
        //CharacterController characterController = stateMachine.Player.GetComponent<CharacterController>();

        if (playercollider != null /*&& characterController != null*/)
        {
            playercollider.enabled = false;
            //characterController.enabled = false;
        }
    }

    private void EnablePlayerCollider()
    {
        CapsuleCollider playercollider = stateMachine.Player.GetComponent<CapsuleCollider>();
        CharacterController characterController = stateMachine.Player.GetComponent<CharacterController>();

        if (playercollider != null && characterController != null)
        {
            playercollider.enabled = true;
            characterController.enabled = true;
        }
    }

    public override void Update()
    {
        base.Update();
        float normalizedTime = GetNormalizedTime(stateMachine.Player.Animator, "Roll");
        if (normalizedTime < 1f)
        {
            return;
        }
        else
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
