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

        float normalizedTime = GetNormalizedTime("Shield");
        if (normalizedTime <= 0.9f)
        {
            return;
        }
        else
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
