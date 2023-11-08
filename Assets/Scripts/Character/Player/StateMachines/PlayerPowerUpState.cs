using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUpState : PlayerGroundState
{
    private float powerUpStartTime;
    private Buff buff;
    public PlayerPowerUpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log(stateMachine.MovementSpeedModifier);
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.PowerUpParameterHash);
        stateMachine.Player.Playerconditions.ActivePowerUp(groundData.PowerUpCost);
        powerUpStartTime = Time.time;
        buff = new Buff(BuffTypes.speed, stateMachine);  // ����Ÿ���� ��ü�� ���� ������ �޸� �ּҸ� ����.
        buff.ApplyBuff(10);
        stateMachine.buffs.Add(buff);
        isMovable = false;
    }


    public override void Update()
    {
        base.Update();

        
        // �ִϸ��̼� �̸��� "PowerUp"�̰� �ִϸ��̼��� ������ �� ���¸� ����
        if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("PowerUp") &&
            stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.PowerUpParameterHash);
    }

    

}
