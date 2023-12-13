using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerGroundState
{
    public PlayerWalkState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.WalkParameterHash);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "NewWalk", stateMachine.Player.transform, true, 1f, 0.04f);
        stateMachine.MovementSpeed = groundData.WalkSpeed;
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.WalkParameterHash);
        soundManager.CallStopLoopSFX(ClipType.PlayerSFX, "NewWalk");
    }
    public override void Update()
    {
        base.Update();
        OnIdle();
    }
    protected override void OnRunStarted(InputAction.CallbackContext context)
    {
        base.OnRunStarted(context);
        stateMachine.ChangeState(stateMachine.RunState);
    }
}
