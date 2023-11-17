using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "AOEActionSO", menuName = "Characters/Enemy/AttackAction/AOEAction")]

public class AOEAction : AttackAction
{
    private ParticleSystem particle;
    private ParticleMediator breathParticle;
    private AOEIndicator indicator;
    private Vector3 initialPosition;

    [Header("Particle")]
    [Tooltip("�극���� ���� ��ƼŬ�Դϴ�.")]
    public ParticleSystem BreathParticlePrefab;
    [Tooltip("�극�� ��ƼŬ�� �ӷ��Դϴ�. �������� ���ڰ� ������ ���ư��ϴ�.")]
    [Range(1f, 10f)] public float StartSpeed = 5f; 
    [Tooltip("�극�� ��ƼŬ�� ũ���Դϴ�")]
    [Range(0.1f, 5f)] public float StartSize = 5f; 
    [Tooltip("��Ƴ� ��ƼŬ ���Դϴ�. �ʹ� ������ ���� �� �ֽ��ϴ�.")]
    [Range(300, 1000)] public int MaxParticle = 300;
    [Tooltip("��ƼŬ�� �����Ǵ� ��ġ �������Դϴ�.")]
    public Vector3 ParticlePositionOffeset;
    public AOEAction() : base()
    {
        ActionType = ActionTypes.AoE;
    }
    public override void OnAwake()
    {
        base.OnAwake();
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled += AnimEventDecision;
        InitializeParticle();
        SetParticleMediator();
    }

    public override void OnStart()
    {
        base.OnStart();
        StartAnimation(Config.AnimTriggerHash1);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed)
        {
            isCompleted = true;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }

    private void AnimEventDecision(AnimationEvent animEvent)
    {
        if (!isRunning)
            return;

        if (animEvent.stringParameter == "BreathStart")
        {
            particle.Play();
        }
        else if (animEvent.stringParameter == "BreathEnd")
        {
            particle.Stop();
        }
        else if (animEvent.stringParameter == "AOEIndicatorOn")
        {
            if (Config.AOEType == AOETypes.None)
            {
                Debug.LogError("AOEŸ���� None�Դϴ�.");
                return;
            }
            indicator = AOEIndicatorPool.Instance.GetIndicatorPool(Config.AOEType).Get();
            indicator.IndicateAOE(particle.transform.position, StateMachine.Enemy.transform.forward, Condition.LessThanThisDistance);
        }
        else if (animEvent.stringParameter == "AOEIndicatorOff")
        {
            if (indicator == null)
                return;
            AOEIndicatorPool.Instance.GetIndicatorPool(Config.AOEType).Release(indicator);
        }
    }
    private void InitializeParticle()
    {
        Transform enemyTransform = StateMachine.Enemy.transform;
        initialPosition = enemyTransform.position + enemyTransform.forward * StateMachine.Enemy.Agent.radius + ParticlePositionOffeset;
        particle = Instantiate(BreathParticlePrefab, initialPosition, Quaternion.identity, enemyTransform);
        //_particle.transform.parent = enemyTransform;
        ParticleSystem.MainModule main = particle.main;
        ParticleSystem.ShapeModule shape = particle.shape;


        main.startLifetime = Condition.LessThanThisDistance / StartSpeed;
        main.maxParticles = MaxParticle;
        main.startSize = StartSize;

        switch (Config.AOEType)
        {
            case AOETypes.Circle45:
                shape.angle = 45 / 2;
                break;
            case AOETypes.Circle90:
                shape.angle = 90 / 2;
                break;
            default:
                Debug.LogError("�극���� AOE������ 45 or 90������ �մϴ�.");
                break;
        }
    }
    private void SetParticleMediator()
    {
        if (!particle.TryGetComponent(out breathParticle))
        {
            Debug.LogError("��ƼŬ �����տ� BreathParticle ������Ʈ�� �����ϴ�.");
            return;
        }
        breathParticle.OnCollisionOccured += OnParticleCollision;
    }
    private void OnParticleCollision(GameObject other)
    {
        ApplyAttack(other);
    }
}
