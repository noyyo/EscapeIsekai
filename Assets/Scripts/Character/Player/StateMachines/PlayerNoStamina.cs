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
        stateMachine.Player.Playerconditions.UseNoStamina(groundData.NoStaminaCost);
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

        float normalizedTime = GetNormalizedTime(stateMachine.Player.Animator, "NoStamina");
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
