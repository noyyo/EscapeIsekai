public class PlayerNothingState : PlayerBaseState
{
    public PlayerNothingState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isMovable = false;
        StartAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        isMovable = true;
        StopAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
    }

}
