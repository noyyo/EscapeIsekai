using UnityEngine;

public class PlayerShieldState : PlayerGroundState
{
    private float powerUpStartTime;
    private Buff buff;
    private float normalizedTime;


    public PlayerShieldState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Player.Playerconditions.UseShield(groundData.ShieldCost);
        StartAnimation(stateMachine.Player.AnimationData.ShieldParameterHash);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "Shield", stateMachine.Player.transform, false);
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
        soundManager.CallStopLoopSFX(ClipType.PlayerSFX, "Shield");
        isMovable = true;
    }

    public override void Update()
    {
        base.Update();
        normalizedTime = GetNormalizedTime("Shield");
        if (normalizedTime >= 1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }
}
