using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Recorder.AOV;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "AOEActionSO", menuName = "Characters/Enemy/AttackAction/Breath")]

public class Breath : AttackAction
{
    private ParticleSystem particle;
    private ParticleMediator breathParticle;
    private AOEIndicator indicator;
    private Vector3 initialPosition;
    [Header("브레스가 쏘아질 AOE 입니다. 30또는 45가 아니라면 오류가 발생합니다.")]
    [SerializeField] private AOETypes aoeType;

    [Header("파티클 설정")]
    [Tooltip("브레스에 쓰일 파티클입니다.")]
    [SerializeField] private ParticleSystem breathParticlePrefab;
    [Tooltip("브레스 파티클의 속력입니다. 높을수록 입자가 빠르게 날아갑니다.")]
    [SerializeField][Range(1f, 10f)] private float startSpeed = 5f; 
    [Tooltip("브레스 파티클의 크기입니다")]
    [SerializeField][Range(0.1f, 5f)] private float startSize = 5f; 
    [Tooltip("쏘아낼 파티클 수입니다. 너무 적으면 비어보일 수 있습니다.")]
    [SerializeField][Range(300, 1000)] private int maxParticle = 300;
    [Tooltip("파티클을 쏘아낼 수직방향 각도입니다.")]
    [SerializeField][Range(15f, 45f)] private float verticalAngle = 30f;
    [Tooltip("파티클이 생성되는 위치 오프셋입니다.")]
    [SerializeField] private Vector3 offset;
    public Breath() : base()
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
            if (aoeType == AOETypes.None)
            {
                Debug.LogError("AOE타입이 None입니다.");
                return;
            }
            indicator = AOEIndicatorPool.Instance.GetIndicatorPool(aoeType).Get();
            indicator.IndicateCircleAOE(particle.transform.position, StateMachine.Enemy.transform.forward, Condition.LessThanThisDistance);
        }
        else if (animEvent.stringParameter == "AOEIndicatorOff")
        {
            if (indicator == null)
                return;
            AOEIndicatorPool.Instance.GetIndicatorPool(aoeType).Release(indicator);
        }
    }
    private void InitializeParticle()
    {
        Transform enemyTransform = StateMachine.Enemy.transform;
        initialPosition = enemyTransform.position + offset;
        particle = Instantiate(breathParticlePrefab, initialPosition, Quaternion.identity, enemyTransform);
        ParticleSystem.MainModule main = particle.main;
        ParticleSystem.ShapeModule shape = particle.shape;


        main.startLifetime = Condition.LessThanThisDistance / startSpeed;
        main.maxParticles = maxParticle;
        main.startSize = startSize;

        switch (aoeType)
        {
            case AOETypes.Circle30:
                shape.angle = 30;
                break;
            case AOETypes.Circle45:
                shape.angle = 45;
                break;
            default:
                Debug.LogError("브레스의 AOE각도는 30 or 45도여야 합니다.");
                break;
        }
        float verticalScale = verticalAngle / shape.angle;
        shape.scale = new Vector3(1, verticalScale, 1);
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
    protected override void OnDrawGizmo(Transform enemyTransform)
    {
        Gizmos.color = Color.red;
        Vector3 startPosition = enemyTransform.position + offset;
        Quaternion verticalRotation = Quaternion.Euler(0, verticalAngle, 0);
        Vector3 direction = verticalRotation * enemyTransform.forward;
        Gizmos.DrawRay(startPosition, direction * Condition.LessThanThisDistance);
    }
}
