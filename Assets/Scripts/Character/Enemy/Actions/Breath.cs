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
    private PointReference breathReference;
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
    [Tooltip("브레스를 쏘아낼 수직방향 각도입니다.")]
    [SerializeField][Range(-30f, 30f)] private float breathVerticalAngle = 0;
    [Tooltip("브레스의 수직방향을 타겟에 따라 조정할지 여부입니다. true라면 breathVerticalAngle은 무시됩니다.")]
    [SerializeField] private bool isTargeting;
    [Tooltip("브레스 시작점에서 파티클을 쏘아낼 수직방향 각도입니다.")]
    [SerializeField][Range(15f, 45f)] private float particleVerticalAngle = 30f;

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
            Vector3 indicatorPosition = StateMachine.Enemy.transform.position;
            indicatorPosition.y += StateMachine.Enemy.transform.InverseTransformPoint(particle.transform.position).y;
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
        breathReference = StateMachine.Enemy.PointReferences[PointReferenceTypes.Breath];
        if (isTargeting)
            breathVerticalAngle = 0;
        particle = Instantiate(breathParticlePrefab, breathReference.transform);
        particle.transform.localRotation = Quaternion.Euler(breathVerticalAngle, 0, 0);
        ParticleSystem.MainModule main = particle.main;
        ParticleSystem.ShapeModule shape = particle.shape;


        main.startLifetime = Condition.LessThanThisDistance / startSpeed;
        main.maxParticles = maxParticle;
        ParticleSystem.EmissionModule emission = particle.emission;
        emission.rateOverTime = (int)(maxParticle / main.startLifetime.constant);
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
        shape.rotation = Vector3.zero;
        float verticalScale = particleVerticalAngle / shape.angle;
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
    private void AdjustBreathVerticalAngle()
    {
        if (!isTargeting)
            return;
        IPositionable target = StateMachine.PositionableTarget;
        Vector3 direction = target.GetObjectCenterPosition() - breathReference.transform.position;
        direction.Normalize();
        breathVerticalAngle = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        Mathf.Clamp(breathVerticalAngle, -30f, 30f);
        Quaternion particleRotation = Quaternion.Euler(breathVerticalAngle, 0, 0);
        particle.transform.localRotation = particleRotation;
    }
    private void OnParticleCollision(GameObject other)
    {
        ApplyAttack(other);
    }
    protected override void OnDrawGizmo(Transform enemyTransform)
    {
        //Gizmos.color = Color.red;
        //Vector3 startPosition = enemyTransform.position;
        //Quaternion verticalRotation = Quaternion.Euler(0, particleVerticalAngle, 0);
        //Vector3 direction = verticalRotation * enemyTransform.forward;
        //Gizmos.DrawRay(startPosition, direction * Condition.LessThanThisDistance);
    }
}
