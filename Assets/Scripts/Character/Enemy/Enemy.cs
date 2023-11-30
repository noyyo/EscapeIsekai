using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IPositionable
{
    public event Action<Collision> OnCollisionOcurred;
    [field: SerializeField] public EnemySO Data { get; private set; }
    [field: SerializeField] public EnemyAnimationData AnimationData { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    private new Collider collider;
    [field: SerializeField] public EnemyStateMachine StateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    [SerializeField] private AffectedAttackEffectInfo affectedEffectInfo;

    public  AttackAction[] Actions;
    public EnemyForceReceiver ForceReceiver { get; private set; }
    public AnimationEventReceiver AnimEventReceiver { get; private set; }
    public Dictionary<PointReferenceTypes, PointReference> PointReferences { get; private set; }

    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        AnimationData = new EnemyAnimationData();
        Rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        Agent = GetComponent<NavMeshAgent>();
        ForceReceiver = GetComponent<EnemyForceReceiver>();
        AnimEventReceiver = GetComponentInChildren<AnimationEventReceiver>();
        PointReferences = new Dictionary<PointReferenceTypes, PointReference>();
        foreach (PointReference point in GetComponentsInChildren<PointReference>())
        {
            PointReferences.Add(point.PointType, point);
        }
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
    private void InitializePointReference()
    {

    }
    public void ResetEnemy()
    {
        StateMachine.ResetStateMachine();
    }
    public void OnRelease()
    {
        gameObject.SetActive(false);
        Agent.enabled = false;
        transform.position = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionOcurred?.Invoke(collision);
    }

    public Vector3 GetObjectCenterPosition()
    {
        return collider.bounds.center;
    }
}