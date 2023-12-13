public class PlayerRunState : PlayerGroundState
{
    public PlayerRunState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.RunParameterHash);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "Run", stateMachine.Player.transform, true, 1f, 0.04f);
        stateMachine.MovementSpeed = groundData.RunSpeed;
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.RunParameterHash);
        soundManager.CallStopLoopSFX(ClipType.PlayerSFX, "Run");
    }
    public override void Update()
    {
        base.Update();
        OnIdle();
    }
}
