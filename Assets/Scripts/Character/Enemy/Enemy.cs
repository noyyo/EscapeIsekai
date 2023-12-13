using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IPositionable
{
    public event Action<Collision> OnCollisionOcurred;
    public event Action<Collider> OnTriggerEntered;
    [field: SerializeField] public EnemySO Data { get; protected set; }
    [field: SerializeField] public EnemyAnimationData AnimationData { get; protected set; }
    public Animator Animator { get; protected set; }
    public Rigidbody Rigidbody { get; protected set; }
    [HideInInspector] public Collider Collider;
    [field: SerializeField] public EnemyStateMachine StateMachine { get; protected set; }
    public NavMeshAgent Agent { get; protected set; }
    [SerializeField] protected AffectedAttackEffectInfo affectedEffectInfo;

    public AttackAction[] Actions;
    public EnemyForceReceiver ForceReceiver { get; protected set; }
    public AnimationEventReceiver AnimEventReceiver { get; protected set; }
    public Dictionary<PointReferenceTypes, PointReference> PointReferences { get; protected set; }
    public Dictionary<int, Weapon> Weapons;
    protected void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        AnimationData = new EnemyAnimationData();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
        Agent = GetComponent<NavMeshAgent>();
        ForceReceiver = GetComponent<EnemyForceReceiver>();
        AnimEventReceiver = GetComponentInChildren<AnimationEventReceiver>();
        PointReferences = new Dictionary<PointReferenceTypes, PointReference>();
        foreach (PointReference point in GetComponentsInChildren<PointReference>())
        {
            PointReferences.Add(point.PointType, point);
        }
        Weapons = new Dictionary<int, Weapon>();
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>())
        {
            Weapons.Add(weapon.ID, weapon);
        }
        StateMachine = new EnemyStateMachine(this);
        Init();
    }

    protected void Start()
    {
        StateMachine.ChangeState(StateMachine.IdleState);
    }

    protected void Update()
    {
        StateMachine.Update();
    }

    protected void FixedUpdate()
    {
        StateMachine.PhysicsUpdate();
    }

    protected void Init()
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
    protected void InitializePointReference()
    {

    }
    public Enemy OnGet()
    {
        Collider.enabled = true;
        enabled = true;
        StateMachine.ResetStateMachine();
        return this;
    }
    public void OnRelease()
    {
        gameObject.SetActive(false);
        Agent.enabled = false;
        transform.position = Vector3.zero;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        OnCollisionOcurred?.Invoke(collision);
    }

    public Vector3 GetObjectCenterPosition()
    {
        return Collider.bounds.center;
    }
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEntered?.Invoke(other);
    }
}