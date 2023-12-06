using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBaseState : IState
{

    // 모든 State는 StateMachine과 역참조를 함.
    protected PlayerStateMachine stateMachine;
    protected readonly PlayerGroundData groundData;
    protected bool isMovable = true;
    protected bool forceMove = false;
    protected CharacterController controller;
    protected Animator animator;
    private bool isAnimStarted;
    protected SoundManager soundManager;

    public PlayerBaseState(PlayerStateMachine playerstateMachine)
    {
        stateMachine = playerstateMachine;
        groundData = stateMachine.Player.Data.GroundedData;
        controller = playerstateMachine.Player.Controller;
        animator = playerstateMachine.Player.Animator;
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
        return forward * stateMachine.MovementInput.y + right * stateMachine.MovementInput.x;
    }

    private void Move(Vector3 movementDirection)
    {
        if (!isMovable) return;
        // 플레이어 이동처리
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
}
