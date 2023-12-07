public class PlayerSuperJump : PlayerAirState
{
    public PlayerSuperJump(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {

        base.Enter();

        stateMachine.JumpForce = stateMachine.Player.Data.AirData.JumpForce;
        stateMachine.Player.ForceReceiver.Jump(10);
        StartAnimation(stateMachine.Player.AnimationData.SuperJumpParameterHash);
        stateMachine.Player.Playerconditions.UseSuperJump(groundData.SuperJumpCost);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "Jump", stateMachine.Player.transform, false, 1f, 0.1f);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.SuperJumpParameterHash);
        soundManager.CallStopLoopSFX(ClipType.PlayerSFX, "Jump");
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (stateMachine.Player.Controller.velocity.y <= 0)
        {
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }
    }
}
