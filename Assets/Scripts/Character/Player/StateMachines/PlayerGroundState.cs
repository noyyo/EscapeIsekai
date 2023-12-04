using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        // ground�� ��ӹ޴� �κ��� ground�� bool ���� �����ִ�ä�� ������ ��.
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
        // Player�� ���� �����ʰ� �������� �ִ� ���¶�� FallState�� ����
        // �߷��� �ѹ��� �޴� ���� �� ũ��(�������� ���¸� �ǹ�)
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
        if (stateMachine.Player.Playerconditions.throwskill.curValue < groundData.ThrowCost)
            return;
        if (GameManager.Instance.Ui_Manager.questManager.GetComponent<QuestManager>().questCheck[0])
            stateMachine.ChangeState(stateMachine.ThrowState);
    }

    protected override void OnNoStaminaStarted(InputAction.CallbackContext context)
    {
        if (stateMachine.Player.Playerconditions.noStamina.curValue < groundData.NoStaminaCost)
            return;
        if (GameManager.Instance.Ui_Manager.questManager.GetComponent<QuestManager>().questCheck[1])
            stateMachine.ChangeState(stateMachine.NoStamina);
    }

    protected override void OnShieldStarted(InputAction.CallbackContext context)
    {
        if (stateMachine.Player.Playerconditions.shield.curValue < groundData.ShieldCost)
            return;
        if (GameManager.Instance.Ui_Manager.questManager.GetComponent<QuestManager>().questCheck[2])
            stateMachine.ChangeState(stateMachine.ShieldState);
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
        if (stateMachine.Player.Playerconditions.rollCoolTime.curValue < groundData.RollCoolTime)
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
