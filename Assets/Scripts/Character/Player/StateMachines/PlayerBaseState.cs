using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class PlayerBaseState : IState
{

    // 모든 State는 StateMachine과 역참조를 함.
    protected PlayerStateMachine stateMachine;
    protected readonly PlayerGroundData groundData;
    protected bool isMovable = true;

    public PlayerBaseState(PlayerStateMachine playerstateMachine)
    {
        stateMachine = playerstateMachine;
        groundData = stateMachine.Player.Data.GroundedData;
    }

    public virtual void Enter()
    {
        AddInputActionsCallbacks();
    }

    public virtual void Exit()
    {
        RemoveInputActionsCallbacks();
    }

    public virtual void HandleInput()
    {
        ReadMovementInput();
    }

    public virtual void PhysicsUpdate()
    {
        
    }

    public virtual void Update()
    {
        Move();
    }

    //키 입력처리 부분
    protected virtual void AddInputActionsCallbacks()
    {
        PlayerInputSystem input = stateMachine.Player.Input;
        input.PlayerActions.Movement.canceled += OnMoveCanceled;
        input.PlayerActions.Run.started += OnRunStarted;

        input.PlayerActions.Jump.started += OnJumpStarted;

        input.PlayerActions.Attack.performed += OnAttackPerformed;
        input.PlayerActions.Attack.canceled += OnAttackCanceled;

        input.PlayerActions.SuperJump.started += OnSuperJumpStarted;
    }


    protected virtual void RemoveInputActionsCallbacks()
    {

        PlayerInputSystem input = stateMachine.Player.Input;
        input.PlayerActions.Movement.canceled -= OnMoveCanceled;
        input.PlayerActions.Run.started -= OnRunStarted;


        input.PlayerActions.Jump.started -= OnJumpStarted;

        input.PlayerActions.Attack.performed -= OnAttackPerformed;
        input.PlayerActions.Attack.canceled -= OnAttackCanceled;

        input.PlayerActions.SuperJump.started -= OnSuperJumpStarted;
    }


    // move와 run을 callback 함수로 받아서 정의
    protected virtual void OnMoveCanceled(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnRunStarted(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnJumpStarted(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnAttackPerformed(InputAction.CallbackContext context)
    {
        stateMachine.IsAttacking = true;
    }
    protected virtual void OnAttackCanceled(InputAction.CallbackContext context)
    {
        stateMachine.IsAttacking = false;
    }
    protected virtual void OnSuperJumpStarted(InputAction.CallbackContext context)
    {
        Debug.Log("count");
        if (stateMachine.Player.Controller.isGrounded)
            stateMachine.ChangeState(stateMachine.SuperJump);
    }

    private void ReadMovementInput()
    {
        // 역참조. 자주사용하는 것들은 캐싱을 해놓는 것이 좋다.
        stateMachine.MovementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
    }

    //실제 이동 처리
    private void Move()
    {
        Vector3 movementDirection = GetMovementDirection();

        Rotate(movementDirection);

        Move(movementDirection);
    }

    private Vector3 GetMovementDirection()
    {
        // 카메라가 바라보는 방향으로 이동하게끔 구현
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        // y값을 제거해야 땅바닥을 보지 않는다.
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // 이동해야 하는 벡터에 입력한 이동방향을 곱해야 이동 처리가 이루어진다.
        return forward* stateMachine.MovementInput.y + right * stateMachine.MovementInput.x;
    }

    private void Move(Vector3 movementDirection)
    {
        if(!isMovable) return;
        // 플레이어 이동처리
        float movementSpeed = GetMovementSpeed();
        stateMachine.Player.Controller.Move(
            ((movementDirection * movementSpeed)
            + stateMachine.Player.ForceReceiver.Movement) 
            *Time.deltaTime
            );
    }

    protected void ForceMove()
    {
        stateMachine.Player.Controller.Move(stateMachine.Player.ForceReceiver.Movement * Time.deltaTime);
    }

    private void Rotate(Vector3 movementDirection)
    {
        if(movementDirection != Vector3.zero)
        {
            Transform playerTransform = stateMachine.Player.transform;
            // 바라보는 방향으로 회전을 하게끔 구현
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, stateMachine.RotationDamping * Time.deltaTime);
        }
    }

    private float GetMovementSpeed()
    {
        // 실제로 이동하는 속도의 처리
        float movementSpeed = stateMachine.MovementSpeed * stateMachine.MovementSpeedModifier;
        return movementSpeed;
    }

    // state 마다 애니메이션을 추가하는 처리
    // SetBool 을 통해 애니메이션을 재생하고 끝내는 로직
    protected void StartAnimation(int animationHash)
    {
        stateMachine.Player.Animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        stateMachine.Player.Animator.SetBool(animationHash, false);
    }

    protected float GetNormalizedTime(Animator animator, string tag)
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if(animator.IsInTransition(0) && nextInfo.IsTag(tag))
        {
            return nextInfo.normalizedTime;
        }
        else if(!animator.IsInTransition(0) && currentInfo.IsTag(tag))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }
}
