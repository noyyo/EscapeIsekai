using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    public event Action<AnimationEvent> AnimationEventCalled;
    public event Action OnDie;

    [field: SerializeField] public EnemySO Data { get; private set; }
    [field: SerializeField] public EnemyAnimationData AnimationData { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public CharacterController Controller { get; private set; }
    [field: SerializeField] public EnemyStateMachine stateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public int HP { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool CanTakeAttackEffect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public  AttackAction[] Actions;
    // Test
    public GameObject Player;
    void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        AnimationData = new EnemyAnimationData();
        Rigidbody = GetComponent<Rigidbody>();
        Controller = GetComponent<CharacterController>();
        Agent = GetComponent<NavMeshAgent>();
        stateMachine = new EnemyStateMachine(this);
        Init();
    }

    private void Start()
    {
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

    private void Init()
    {
        AnimationData.Initialize();
        Agent.speed = Data.WalkSpeed;
        Agent.angularSpeed = Data.RotateSpeed;
        Agent.acceleration = Data.Acceleration;
        stateMachine.OriginPosition = transform.position;
        // 액션 데이터 복사본 생성
        for (int i = 0; i < Actions.Length; i++)
        {
            Actions[i] = Instantiate(Actions[i]);
            Actions[i].SetStateMachine(stateMachine);
            Actions[i].OnAwake();
        }
    }
    public void OnAnimationEventCalled(AnimationEvent animEvent)
    {
        AnimationEventCalled?.Invoke(animEvent);
    }

    public void TakeDamage(int damage)
    {
        throw new NotImplementedException();
    }
}