using UnityEngine;

public class PlayerPowerUpState : PlayerGroundState
{
    private float powerUpStartTime;
    private Buff buff;
    private float normalizedTime;

    public PlayerPowerUpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.PowerUpParameterHash);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "PowerUp", stateMachine.Player.transform, false, 1f, 0.1f);
        stateMachine.Player.Playerconditions.ActivePowerUp(groundData.PowerUpCost);
        powerUpStartTime = Time.time;
        buff = new Buff(BuffTypes.speed, stateMachine);
        buff.ApplyBuff(10);
        stateMachine.buffs.Add(buff);
        isMovable = false;
    }


    public override void Update()
    {
        base.Update();
        normalizedTime = GetNormalizedTime("PowerUp");
        if (normalizedTime >= 1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.PowerUpParameterHash);
        soundManager.CallStopLoopSFX(ClipType.PlayerSFX, "PowerUp");
        isMovable = true;
    }



}
