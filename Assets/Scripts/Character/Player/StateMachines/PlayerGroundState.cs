using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundState : PlayerBaseState
{
    private bool isSliding;
    private Vector3 slidingVelocity;
    public PlayerGroundState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
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
        SetSlidingVelocity();
        if (isSliding)
        {
            controller.Move((player.ForceReceiver.Movement + slidingVelocity) * Time.deltaTime);
        }
        if (stateMachine.IsAttacking && !isSliding)
        {
            OnAttack();
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    protected void OnIdle()
    {
        if (!isMovable)
        {
            return;
        }
        if (stateMachine.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (isSliding)
            return;
        stateMachine.ChangeState(stateMachine.JumpState);
    }

    protected override void OnSuperJumpStarted(InputAction.CallbackContext context)
    {
        if (isSliding)
            return;
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
        stateMachine.RollState.beforeState = stateMachine.currentState;
        stateMachine.ChangeState(stateMachine.RollState);
    }

    protected virtual void OnSkillStarted(InputAction.CallbackContext context)
    {
        if (stateMachine.Player.Playerconditions.skill.curValue < groundData.SkillCost)
            return;
        stateMachine.ChangeState(stateMachine.SkillState);
    }

    protected virtual void OnPowerUpStarted(InputAction.CallbackContext context)
    {
        if (stateMachine.Player.Playerconditions.powerUp.curValue < groundData.PowerUpCost)
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
    private void SetSlidingVelocity()
    {
        if (Physics.Raycast(player.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, controller.radius * 10))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle > controller.slopeLimit)
            {
                slidingVelocity += Vector3.ProjectOnPlane(new Vector3(0, Physics.gravity.y * Time.deltaTime, 0), hit.normal);
                isSliding = true;
                isMovable = false;
                return;
            }
            
        }
        else // tangent값이 10 이상, 약 경사 85도가량 이상이지만 땅에 닿아있을 때
        {
            Vector3 currentPosition = player.transform.position;
            currentPosition.y -= controller.center.y - controller.height / 2;
            if (Physics.SphereCast(currentPosition, controller.radius, Vector3.down, out RaycastHit sphereHit, 0.1f))
            {
                slidingVelocity += Vector3.ProjectOnPlane(new Vector3(0, Physics.gravity.y * Time.deltaTime, 0), hit.normal);
                isSliding = true;
                isMovable = false;
                return;
            }
        }

        if (isSliding)
        {
            float decelerationRate = Time.deltaTime * 3f;
            if (decelerationRate > 1f)
                slidingVelocity = Vector3.zero;
            else
                slidingVelocity -= slidingVelocity * decelerationRate;
            if (slidingVelocity.sqrMagnitude < 1)
            {
                slidingVelocity = Vector3.zero;
                isSliding = false;
                isMovable = true;
            }
        }

    }
}
