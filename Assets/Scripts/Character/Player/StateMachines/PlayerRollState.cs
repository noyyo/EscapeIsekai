using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRollState : PlayerGroundState
{
    private Vector3 direction;
    private float normalizedTime;
    private AnimationCurve rollCurve;
    public IState beforeState;
    public PlayerRollState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
        rollCurve = groundData.RollCurve;
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.RollParameterHash);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "Roll", stateMachine.Player.transform, false, 1f, 0.1f);
        isMovable = false;
        direction = stateMachine.Player.transform.forward;
        direction.y = 0;
        direction.Normalize();
        if (beforeState == stateMachine.WalkState)
            StartAnimation(stateMachine.Player.AnimationData.WalkParameterHash);
        else if (beforeState == stateMachine.RunState)
            StartAnimation(stateMachine.Player.AnimationData.RunParameterHash);

        stateMachine.Player.Playerconditions.UseStamina(groundData.StaminaCost);
        stateMachine.Player.Playerconditions.RollCoolTime(groundData.RollCoolTime);
    }

    public override void Exit()
    {
        base.Exit();
        isMovable = true;
        normalizedTime = 0f;
        soundManager.CallStopLoopSFX(ClipType.PlayerSFX, "Roll");
    }

    protected override void OnMoveCanceled(InputAction.CallbackContext context)
    {
        base.OnMoveCanceled(context);
        if (beforeState == stateMachine.WalkState)
            StopAnimation(stateMachine.Player.AnimationData.WalkParameterHash);
        else if (beforeState == stateMachine.RunState)
            StopAnimation(stateMachine.Player.AnimationData.RunParameterHash);
        beforeState = null;
    }
    public override void Update()
    {
        base.Update();
        normalizedTime = GetNormalizedTime("Roll");
        if (normalizedTime >= 1f)
        {
            if (beforeState == stateMachine.RunState)
                stateMachine.ChangeState(stateMachine.RunState);
            else if (beforeState == stateMachine.WalkState)
                stateMachine.ChangeState(stateMachine.WalkState);
            else
                stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
        else
        {
            normalizedTime = GetNormalizedTime("Roll");
            controller.Move((direction * rollCurve.Evaluate(normalizedTime) + stateMachine.Player.ForceReceiver.Movement) * stateMachine.MovementSpeedModifier * Time.deltaTime);
        }
    }
}
