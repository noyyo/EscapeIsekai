using UnityEngine;

public class PlayerNoStamina : PlayerGroundState
{
    private float powerUpStartTime;
    private Buff buff;
    private float normalizedTime;

    public PlayerNoStamina(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.NoStaminaParameterHash);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "NoStamina", stateMachine.Player.transform, false, 1f, 0.1f);
        stateMachine.Player.Playerconditions.UseNoStamina(groundData.NoStaminaCost);
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
        soundManager.CallStopLoopSFX(ClipType.PlayerSFX, "NoStamina");
        isMovable = true;
    }

    public override void Update()
    {
        base.Update();
        normalizedTime = GetNormalizedTime("NoStamina");
        if (normalizedTime >= 1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }
}
