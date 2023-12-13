using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBaseState : IState
{
    protected PlayerStateMachine stateMachine;
    protected CharacterController controller;
    protected SoundManager soundManager;
    protected Player player;
    protected Animator animator;
    protected readonly PlayerGroundData groundData;
    protected bool isMovable = true;
    protected bool forceMove = false;
    private bool isAnimStarted;

    public PlayerBaseState(PlayerStateMachine playerStateMachine)
    {
        stateMachine = playerStateMachine;
        player = playerStateMachine.Player;
        groundData = stateMachine.Player.Data.GroundedData;
        controller = playerStateMachine.Player.Controller;
        animator = playerStateMachine.Player.Animator;
        if (soundManager == null)
            soundManager = SoundManager.Instance;
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

    protected virtual void AddInputActionsCallbacks()
    {
        PlayerInputSystem input = stateMachine.Player.Input;
        input.PlayerActions.Movement.canceled += OnMoveCanceled;
        input.PlayerActions.Run.started += OnRunStarted;
        input.PlayerActions.Jump.started += OnJumpStarted;
        input.PlayerActions.Attack.performed += OnAttackPerformed;
        input.PlayerActions.Attack.canceled += OnAttackCanceled;
        input.PlayerActions.SuperJump.started += OnSuperJumpStarted;
        input.PlayerActions.Throw.started += OnThrowStarted;
        input.PlayerActions.NoStamina.started += OnNoStaminaStarted;
        input.PlayerActions.Shield.started += OnShieldStarted;
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
        input.PlayerActions.Throw.started -= OnThrowStarted;
        input.PlayerActions.NoStamina.started -= OnNoStaminaStarted;
        input.PlayerActions.Shield.started -= OnShieldStarted;
    }


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

    }

    protected virtual void OnThrowStarted(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnNoStaminaStarted(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnShieldStarted(InputAction.CallbackContext context)
    {

    }


    private void ReadMovementInput()
    {
        stateMachine.MovementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
    }

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
        return forward * stateMachine.MovementInput.y + right * stateMachine.MovementInput.x;
    }

    private void Move(Vector3 movementDirection)
    {
        if (!isMovable) return;
        float movementSpeed = GetMovementSpeed();
        controller.Move(
            ((movementDirection * movementSpeed)
            + stateMachine.Player.ForceReceiver.Movement)
            * Time.deltaTime
            );
    }

    protected void ForceMove()
    {
        stateMachine.Player.Controller.Move(stateMachine.Player.ForceReceiver.Movement * Time.deltaTime);
    }

    private void Rotate(Vector3 movementDirection)
    {
        if (!isMovable) return;
        if (movementDirection != Vector3.zero)
        {
            Transform playerTransform = stateMachine.Player.transform;
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, stateMachine.RotationDamping * Time.deltaTime);
        }
    }

    private float GetMovementSpeed()
    {
        float movementSpeed = stateMachine.MovementSpeed * stateMachine.MovementSpeedModifier;
        return movementSpeed;
    }

    protected void StartAnimation(int animationHash)
    {
        stateMachine.Player.Animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        stateMachine.Player.Animator.SetBool(animationHash, false);
    }

    protected float GetNormalizedTime(string tag)
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (currentInfo.IsTag(tag))
        {
            isAnimStarted = true;
            return currentInfo.normalizedTime;
        }
        else if (isAnimStarted && !currentInfo.IsTag(tag))
        {
            isAnimStarted = false;
            return 1f;
        }
        else
        {
            return 0f;
        }
    }

    protected float GetNormalizedTimeforCombo(Animator animator, string tag)
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag))
        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }
}
