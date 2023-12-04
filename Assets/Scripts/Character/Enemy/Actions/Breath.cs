using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Breath", menuName = "Characters/Enemy/AttackAction/Breath")]

public class Breath : AttackAction
{
    private ParticleSystem particle;
    private ParticleMediator breathParticle;
    private AOEIndicator indicator;
    private PointReference breathReference;
    [Header("�극���� ����� AOE �Դϴ�. 30�Ǵ� 45�� �ƴ϶�� ������ �߻��մϴ�.")]
    [SerializeField] private AOETypes aoeType;

    [Header("��ƼŬ ����")]
    [Tooltip("�극���� ���� ��ƼŬ�Դϴ�.")]
    [SerializeField] private ParticleSystem breathParticlePrefab;
    [Tooltip("�극�� ��ƼŬ�� �ӷ��Դϴ�. �������� ���ڰ� ������ ���ư��ϴ�.")]
    [SerializeField][Range(1f, 10f)] private float startSpeed = 5f;
    [Tooltip("�극�� ��ƼŬ�� ũ���Դϴ�")]
    [SerializeField][Range(0.1f, 5f)] private float startSize = 5f;
    [Tooltip("��Ƴ� ��ƼŬ ���Դϴ�. �ʹ� ������ ���� �� �ֽ��ϴ�. Particle�� Burst��� 0���� �����մϴ�.")]
    [SerializeField][Range(0, 1000)] private int maxParticle = 300;
    [Tooltip("�극���� ��Ƴ� �������� �����Դϴ�.")]
    [SerializeField][Range(-30f, 30f)] private float breathVerticalAngle = 0;
    [Tooltip("�극���� ���������� Ÿ�ٿ� ���� �������� �����Դϴ�. true��� breathVerticalAngle�� ���õ˴ϴ�.")]
    [SerializeField] private bool isTargeting;
    [Tooltip("�극�� ���������� ��ƼŬ�� ��Ƴ� �������� �����Դϴ�.")]
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
        if (maxParticle != 0)
            main.maxParticles = maxParticle;
        ParticleSystem.EmissionModule emission = particle.emission;
        if (maxParticle != 0)
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
            Debug.LogError("��ƼŬ �����տ� ParticleMediator ������Ʈ�� �����ϴ�.");
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
