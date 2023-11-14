using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : PlayerGroundState
{
    public PlayerSkillState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isMovable = false;
        StartAnimation(stateMachine.Player.AnimationData.SkillParameterHash);
        stateMachine.Player.Playerconditions.UseSkill(groundData.SkillCost);
    }

    public override void Exit() 
    { 
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.SkillParameterHash);
    }

    public override void Update()
    {
        base.Update();

        // 애니메이션 이름이 "Skill"이고 애니메이션이 끝났을 때 상태를 변경
        if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill") &&
            stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
