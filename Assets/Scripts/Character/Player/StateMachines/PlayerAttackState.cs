public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isMovable = false;
        StartAnimation(stateMachine.Player.AnimationData.AttackParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        isMovable = true;
        StopAnimation(stateMachine.Player.AnimationData.AttackParameterHash);
    }

}
