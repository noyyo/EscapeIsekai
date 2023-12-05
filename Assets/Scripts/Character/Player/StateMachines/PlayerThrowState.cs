using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerThrowState : PlayerGroundState
{
    private float normalizedTime;
    public PlayerThrowState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("´øÁö±â");
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
        isMovable = true;
    }

    public override void Update()
    {
        base.Update();
        
        normalizedTime = GetNormalizedTime("Throw");
        if (normalizedTime >= 1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }


}
