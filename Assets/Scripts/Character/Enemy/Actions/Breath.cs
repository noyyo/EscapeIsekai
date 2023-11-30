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

[CreateAssetMenu(fileName = "Breath", menuName = "Characters/Enemy/AttackAction/Breath")]

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
    [Tooltip("�극���� ��Ƴ� �������� �����Դϴ�.")]
    [SerializeField][Range(-30f, 30f)] private float breathVerticalAngle = 0;
    [Tooltip("�극���� ���������� Ÿ�ٿ� ���� �������� �����Դϴ�. true��� breathVerticalAngle�� ���õ˴ϴ�.")]
    [SerializeField] private bool isTargeting;
    [Tooltip("�극�� ���������� ��ƼŬ�� ��Ƴ� �������� �����Դϴ�.")]
    [SerializeField][Range(15f, 45f)] private float particleVerticalAngle = 30f;
    [Tooltip("��ƼŬ�� �����Ǵ� ��ġ �������Դϴ�.")]
    [SerializeField] private Vector3 offset;
    public Breath() : base()
    {
        ActionType = ActionTypes.Breath;
    }
    public override void OnAwake()
    {
        base.OnAwake();
        InitializeParticle();
        SetParticleMediator();
    }

    public override void OnStart()
    {
        base.OnStart();
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled += AnimEventDecision;
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
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled -= AnimEventDecision;
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
            indicator = AOEIndicatorPool.Instance.GetIndicatorPool(aoeType).Get();
            Vector3 indicatorPosition = particle.transform.position;
            if (isTargeting)
            {
                AdjustBreathVerticalAngle();
            }
            float breathHeight = Mathf.Max(Mathf.Sin(-breathVerticalAngle), 0) * Condition.LessThanThisDistance;
            indicatorPosition.y += breathHeight;

            indicator.IndicateCircleAOE(indicatorPosition, StateMachine.Enemy.transform.forward, Condition.LessThanThisDistance, Condition.LessThanThisDistance + breathHeight);
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
        Quaternion particleRotation;
        if (!isTargeting)
            particleRotation = Quaternion.Euler(breathVerticalAngle, 0, 0);
        else
            particleRotation = Quaternion.Euler(0,0,0);
        particle = Instantiate(breathParticlePrefab, initialPosition, particleRotation, enemyTransform);
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
        shape.rotation = Vector3.zero;
        float verticalScale = particleVerticalAngle / shape.angle;
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
    private void AdjustBreathVerticalAngle()
    {
        if (!isTargeting)
            return;
        IPositionable target = StateMachine.PositionableTarget;
        Vector3 direction = target.GetObjectCenterPosition() - StateMachine.Enemy.transform.TransformPoint(offset);
        direction.Normalize();
        breathVerticalAngle = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        Mathf.Clamp(breathVerticalAngle, -30f, 30f);
        Quaternion particleRotation = Quaternion.Euler(breathVerticalAngle, particle.transform.rotation.eulerAngles.y, particle.transform.rotation.eulerAngles.z);
        particle.transform.rotation = particleRotation;
    }
    private void OnParticleCollision(GameObject other)
    {
        ApplyAttack(other);
    }
    protected override void OnDrawGizmo(Transform enemyTransform)
    {
        Gizmos.color = Color.red;
        Vector3 startPosition = enemyTransform.position + offset;
        Quaternion verticalRotation = Quaternion.Euler(0, particleVerticalAngle, 0);
        Vector3 direction = verticalRotation * enemyTransform.forward;
        Gizmos.DrawRay(startPosition, direction * Condition.LessThanThisDistance);
    }
}
