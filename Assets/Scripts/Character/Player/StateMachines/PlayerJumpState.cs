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
        // jump������ ���� velocity.y�� 0���� ū ������ �������� �����̹Ƿ�,
        // ���� ������ ������ velocity.y���� 0���� �۰� �Ǿ�� ��.
        // ���� y���� �������� FallState�� �����ϴ� �����̾�� ��.
        if (stateMachine.Player.Controller.velocity.y <= 0)
        {
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }
    }
}
