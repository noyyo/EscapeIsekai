public class PlayerThrowState : PlayerGroundState
{
    public PlayerThrowState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isMovable = false;
        StartAnimation(stateMachine.Player.AnimationData.ThrowParameterHash);
        stateMachine.Player.CreateGrenadeWithDelay(0.8f);
        stateMachine.Player.Playerconditions.UseThrow(groundData.ThrowCost);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.ThrowParameterHash);
    }

    public override void Update()
    {
        base.Update();
        float normalizedTime = GetNormalizedTime(stateMachine.Player.Animator, "Throw");
        if (normalizedTime <= 0.9f)
        {
            return;
        }
        else
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }


}
