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
        StartAnimation(stateMachine.Player.AnimationData.RollParameterHash);
        TryApplyForce();
        stateMachine.Player.Playerconditions.UseStamina(groundData.StaminaCost);
    }

    public override void Exit()
    {
        base.Exit();
        isMovable = false;
        StopAnimation(stateMachine.Player.AnimationData.RollParameterHash);
    }


    private void TryApplyForce()
    {
        // ForceReceiver�� AddForce�� ����. �ٶ󺸰��ִ� ���鿡�� �з�������.
        stateMachine.Player.ForceReceiver.AddForce(stateMachine.Player.transform.forward * groundData.RollForce);
    }

    public override void Update()
    {
        base.Update();

        // �ִϸ��̼� �̸��� "Roll"�̰� �ִϸ��̼��� ������ �� ���¸� ����
        if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") &&
            stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
