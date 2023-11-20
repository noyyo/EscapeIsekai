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
    [Tooltip("브레스에 쓰일 파티클입니다.")]
    public ParticleSystem BreathParticlePrefab;
    [Tooltip("브레스 파티클의 속력입니다. 높을수록 입자가 빠르게 날아갑니다.")]
    [Range(1f, 10f)] public float StartSpeed = 5f; 
    [Tooltip("브레스 파티클의 크기입니다")]
    [Range(0.1f, 5f)] public float StartSize = 5f; 
    [Tooltip("쏘아낼 파티클 수입니다. 너무 적으면 비어보일 수 있습니다.")]
    [Range(300, 1000)] public int MaxParticle = 300;
    [Tooltip("파티클이 생성되는 위치 오프셋입니다.")]
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
                Debug.LogError("AOE타입이 None입니다.");
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
                Debug.LogError("브레스의 AOE각도는 45 or 90도여야 합니다.");
                break;
        }
    }
    private void SetParticleMediator()
    {
        if (!particle.TryGetComponent(out breathParticle))
        {
            Debug.LogError("파티클 프리팹에 BreathParticle 컴포넌트가 없습니다.");
            return;
        }
        breathParticle.OnCollisionOccured += OnParticleCollision;
    }
    private void OnParticleCollision(GameObject other)
    {
        ApplyAttack(other);
    }
}
