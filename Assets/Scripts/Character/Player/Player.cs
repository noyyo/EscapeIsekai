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
        // �ִϸ��̼� �����ʹ� �׻� �ʱ�ȭ�� ���־�� �Ѵ�
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
        // ���콺 Ŀ���� ������� ��
        Cursor.lockState = CursorLockMode.Locked;
        // ĳ���Ͱ� �� ó�� �����ؾ� �� Idle ���·� ������ֱ�
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
