public class PlayerRollState : PlayerGroundState
{
    public PlayerRollState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isMovable = true;
        StartAnimation(stateMachine.Player.AnimationData.RollParameterHash);
        TryApplyForce();
        stateMachine.Player.Playerconditions.UseStamina(groundData.StaminaCost);
        stateMachine.Player.Playerconditions.RollCoolTime(groundData.RollCoolTime);
    }

    public override void Exit()
    {
        base.Exit();
        isMovable = true;
        StopAnimation(stateMachine.Player.AnimationData.RollParameterHash);
    }


    private void TryApplyForce()
    {
        stateMachine.Player.ForceReceiver.Reset();
        stateMachine.Player.ForceReceiver.AddForce(stateMachine.Player.transform.forward * groundData.RollForce);
    }


    public override void Update()
    {
        base.Update();
        float normalizedTime = GetNormalizedTime(stateMachine.Player.Animator, "Roll");
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
