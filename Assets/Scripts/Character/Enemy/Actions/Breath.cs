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
    [Header("�극���� ����� AOE �Դϴ�. 30�Ǵ� 45�� �ƴ϶�� ������ �߻��մϴ�.")]
    [SerializeField] private AOETypes aoeType;

    [Header("��ƼŬ ����")]
    [Tooltip("�극���� ���� ��ƼŬ�Դϴ�.")]
    [SerializeField] private ParticleSystem breathParticlePrefab;
    [Tooltip("�극�� ��ƼŬ�� �ӷ��Դϴ�. �������� ���ڰ� ������ ���ư��ϴ�.")]
    [SerializeField][Range(1f, 10f)] private float startSpeed = 5f; 
    [Tooltip("�극�� ��ƼŬ�� ũ���Դϴ�")]
    [SerializeField][Range(0.1f, 5f)] private float startSize = 5f; 
    [Tooltip("��Ƴ� ��ƼŬ ���Դϴ�. �ʹ� ������ ���� �� �ֽ��ϴ�.")]
    [SerializeField][Range(300, 1000)] private int maxParticle = 300;
    [Tooltip("��ƼŬ�� ��Ƴ� �������� �����Դϴ�.")]
    [SerializeField][Range(15f, 45f)] private float verticalAngle = 30f;
    [Tooltip("��ƼŬ�� �����Ǵ� ��ġ �������Դϴ�.")]
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
                Debug.LogError("AOEŸ���� None�Դϴ�.");
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
                Debug.LogError("�극���� AOE������ 30 or 45������ �մϴ�.");
                break;
        }
        float verticalScale = verticalAngle / shape.angle;
        shape.scale = new Vector3(1, verticalScale, 1);
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
    protected override void OnDrawGizmo(Transform enemyTransform)
    {
        Gizmos.color = Color.red;
        Vector3 startPosition = enemyTransform.position + offset;
        Quaternion verticalRotation = Quaternion.Euler(0, verticalAngle, 0);
        Vector3 direction = verticalRotation * enemyTransform.forward;
        Gizmos.DrawRay(startPosition, direction * Condition.LessThanThisDistance);
    }
}
