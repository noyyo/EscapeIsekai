using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Header("References")]
    [field: SerializeField] public PlayerSO Data { get; private set; }

    [field: Header("Animations")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerInputSystem Input { get; private set; }
    public CharacterController Controller { get; private set; }
    public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public Weapon Weapon { get; private set; }
    public Playerconditions Playerconditions { get; private set; }
    public Buff Buff { get; private set; }

    public PlayerStateMachine StateMachine;
    [field: SerializeField] public AnimationEventReceiver AnimationEventReceiver { get; private set; }
    private void Awake()
    {
        // 애니메이션 데이터는 항상 초기화를 해주어야 한다
        AnimationData.Initialize();

        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponentInChildren<Animator>();
        Input = GetComponent<PlayerInputSystem>();
        Controller = GetComponent<CharacterController>();
        ForceReceiver = GetComponent<ForceReceiver>();
        Playerconditions = GetComponent<Playerconditions>();

        StateMachine = new PlayerStateMachine(this);
    }

    private void Start()
    {
        // 마우스 커서를 사라지게 함
        Cursor.lockState = CursorLockMode.Locked;
        // 캐릭터가 맨 처음 동작해야 할 Idle 상태로 만들어주기
        StateMachine.ChangeState(StateMachine.IdleState);
        //Health.OnDie += OnDie;
        StateMachine.OnDie += OnDie;
    }

    private void Update()
    {
        StateMachine.HandleInput();
        StateMachine.Update();
    }

    private void FixedUpdate()
    {
        StateMachine.PhysicsUpdate();
    }

    void OnDie()
    {
        Animator.SetTrigger("Die");
        enabled = false;
    }

}
