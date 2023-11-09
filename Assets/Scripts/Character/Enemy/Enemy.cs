using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public event Action<AnimationEvent> AnimationEventCalled;

    [field: SerializeField] public EnemySO Data { get; private set; }
    [field: SerializeField] public EnemyAnimationData AnimationData { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public EnemyStateMachine StateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    [SerializeField] private AffectedAttackEffectInfo affectedEffectInfo;

    public  AttackAction[] Actions;
    public EnemyForceReceiver ForceReceiver { get; private set; }
    // Test
    public GameObject Player;
    void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        AnimationData = new EnemyAnimationData();
        Rigidbody = GetComponent<Rigidbody>();
        Agent = GetComponent<NavMeshAgent>();
        ForceReceiver = GetComponent<EnemyForceReceiver>();
        StateMachine = new EnemyStateMachine(this);
        Init();
    }

    private void Start()
    {
        StateMachine.ChangeState(StateMachine.IdleState);
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void FixedUpdate()
    {
        StateMachine.PhysicsUpdate();
    }

    private void Init()
    {
        AnimationData.Initialize();
        Agent.speed = Data.WalkSpeed;
        Agent.angularSpeed = Data.RotateSpeed;
        Agent.acceleration = Data.Acceleration;
        StateMachine.OriginPosition = transform.position;
        // 액션 데이터 복사본 생성
        for (int i = 0; i < Actions.Length; i++)
        {
            Actions[i] = Instantiate(Actions[i]);
            Actions[i].SetStateMachine(StateMachine);
            Actions[i].OnAwake();
        }
    }
    public void OnAnimationEventCalled(AnimationEvent animEvent)
    {
        AnimationEventCalled?.Invoke(animEvent);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
    }
    private void OnCollisionExit(Collision collision)
    {
        Agent.isStopped = false;
    }
}