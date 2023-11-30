using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldState : PlayerGroundState
{
    private float powerUpStartTime;
    private Buff buff;

    public PlayerShieldState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Player.Playerconditions.UseShield(groundData.ShieldCost);
        StartAnimation(stateMachine.Player.AnimationData.ShieldParameterHash);
        powerUpStartTime = Time.time;
        buff = new Buff(BuffTypes.shield, stateMachine);
        buff.ApplyBuff(10);
        stateMachine.buffs.Add(buff);
        isMovable = false;
    }

    public override void Exit() 
    { 
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.ShieldParameterHash);

    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Shield") &&
            stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
