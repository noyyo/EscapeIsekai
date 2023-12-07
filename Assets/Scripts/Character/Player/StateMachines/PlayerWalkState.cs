using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerGroundState
{
    public PlayerWalkState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.MovementSpeed = groundData.WalkSpeed;
        StartAnimation(stateMachine.Player.AnimationData.WalkParameterHash);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "NewWalk", stateMachine.Player.transform, true, 1f, 0.05f);

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
