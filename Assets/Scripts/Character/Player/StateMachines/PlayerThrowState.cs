using UnityEngine;


public class PlayerThrowState : PlayerGroundState
{
    private float normalizedTime;
    public PlayerThrowState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("������");
        base.Enter();
        isMovable = false;
        StartAnimation(stateMachine.Player.AnimationData.ThrowParameterHash);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "Throw", stateMachine.Player.transform, false, 1f, 0.1f);
        stateMachine.Player.CreateGrenadeWithDelay(0.8f);
        stateMachine.Player.Playerconditions.UseThrow(groundData.ThrowCost);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.ThrowParameterHash);
        soundManager.CallStopLoopSFX(ClipType.PlayerSFX, "Throw");
        isMovable = true;
    }

    public override void Update()
    {
        base.Update();

        normalizedTime = GetNormalizedTime("Throw");
        if (normalizedTime >= 1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }


}
