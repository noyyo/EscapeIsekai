using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerGroundState : PlayerBaseState
{

    public PlayerGroundState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        // ground를 상속받는 부분은 ground의 bool 값이 켜져있는채로 시작을 함.
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.GroundParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.GroundParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if(stateMachine.IsAttacking)
        {
            OnAttack();
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // Player가 땅에 있지않고 떨어지고 있는 상태라면 FallState를 적용
        // 중력을 한번에 받는 양이 더 크다(떨어지는 상태를 의미)
        /*
        if(!stateMachine.Player.Controller.isGrounded 
            && stateMachine.Player.Controller.velocity.y < Physics.gravity.y * Time.fixedDeltaTime)
        {
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }
        */
    }

    protected override void OnMoveCanceled(InputAction.CallbackContext context)
    {
        if(stateMachine.MovementInput == Vector2.zero)
        {
            return;
        }

        stateMachine.ChangeState(stateMachine.IdleState);

        base.OnMoveCanceled(context);
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.JumpState);
    }

    protected override void OnSuperJumpStarted(InputAction.CallbackContext context)
    {
        if (stateMachine.Player.Playerconditions.superJump.curValue < groundData.SuperJumpCost)
            return;
        stateMachine.ChangeState(stateMachine.SuperJump);
    }

    protected override void OnThrowStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.ThrowState);
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        PlayerInputSystem input = stateMachine.Player.Input;
        input.PlayerActions.Roll.started += OnRollStarted;
        input.PlayerActions.Skill.started += OnSkillStarted;
        input.PlayerActions.PowerUp.started += OnPowerUpStarted;
    }
    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        PlayerInputSystem input = stateMachine.Player.Input;
        input.PlayerActions.Roll.started -= OnRollStarted;
        input.PlayerActions.Skill.started -= OnSkillStarted;
        input.PlayerActions.PowerUp.started -= OnPowerUpStarted;
    }


    protected virtual void OnRollStarted(InputAction.CallbackContext context)
    {
        if (stateMachine.Player.Playerconditions.stamina.curValue < groundData.StaminaCost)
            return;
        stateMachine.ChangeState(stateMachine.RollState);
    }

    protected virtual void OnSkillStarted(InputAction.CallbackContext context)
    {
        if(stateMachine.Player.Playerconditions.skill.curValue < groundData.SkillCost)
            return;
        stateMachine.ChangeState(stateMachine.SkillState);
    }

    protected virtual void OnPowerUpStarted(InputAction.CallbackContext context)
    {
        if(stateMachine.Player.Playerconditions.powerUp.curValue < groundData.PowerUpCost)
            return;
        stateMachine.ChangeState(stateMachine.PowerUpState);
    }

    protected virtual void OnMove()
    {
        stateMachine.ChangeState(stateMachine.WalkState);
    }

    protected virtual void OnAttack()
    {
        stateMachine.ChangeState(stateMachine.ComboAttackState);
    }

}
