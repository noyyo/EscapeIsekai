public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.JumpForce = stateMachine.Player.Data.AirData.JumpForce;
        stateMachine.Player.ForceReceiver.Jump(stateMachine.JumpForce);

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.JumpParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.JumpParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // jump상태일 때는 velocity.y가 0보다 큰 값으로 적용중인 상태이므로,
        // 땅에 떨어질 때에는 velocity.y값이 0보다 작게 되어야 함.
        // 따라서 y값이 떨어질때 FallState를 적용하는 구조이어야 함.
        if (stateMachine.Player.Controller.velocity.y <= 0)
        {
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }
    }
}
