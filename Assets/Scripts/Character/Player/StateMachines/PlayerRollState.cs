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
        Debug.Log(stateMachine.MovementSpeedModifier);
        base.Enter();
        isMovable = true;
        StartAnimation(stateMachine.Player.AnimationData.RollParameterHash);
        TryApplyForce();
        stateMachine.Player.Playerconditions.UseStamina(groundData.StaminaCost);
    }

    public override void Exit()
    {
        base.Exit();
        isMovable = true;
        StopAnimation(stateMachine.Player.AnimationData.RollParameterHash);
    }


    private void TryApplyForce()
    {
        // 받고있던 힘을(Force) 리셋하고
        stateMachine.Player.ForceReceiver.Reset();
        // ForceReceiver에 AddForce를 적용. 바라보고있는 정면에서 밀려나도록.
        stateMachine.Player.ForceReceiver.AddForce(stateMachine.Player.transform.forward * groundData.RollForce);
    }

    /*
    public override void Update()
    {
        base.Update();

        float normalizedTime = GetNormalizedTime(stateMachine.Player.Animator, "Roll");
        if (normalizedTime >= 1.0f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
    */

    public override void Update()
    {
        base.Update();

        // 애니메이션 이름이 "Roll"이고 애니메이션이 끝났을 때 상태를 변경
        if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") &&
            stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
