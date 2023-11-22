using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNoStamina : PlayerGroundState
{
    private float powerUpStartTime;
    private Buff buff;

    public PlayerNoStamina(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.NoStaminaParameterHash);
        powerUpStartTime = Time.time;
        buff = new Buff(BuffTypes.nostamina, stateMachine);
        buff.ApplyBuff(10);
        stateMachine.buffs.Add(buff);
        isMovable = false;
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.NoStaminaParameterHash);

    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("NoStamina") &&
            stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
