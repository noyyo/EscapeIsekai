using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "ProjectileRain", menuName = "Characters/Enemy/AttackAction/ProjectileRain")]
public class ProjectileRain : AttackAction
{
    [SerializeField] Projectile projectilePrefab;
    [Tooltip("�� ���� ������ ����ü�� �����Դϴ�. �����Ǵ� �ֱ�� EffectDuration�� ���� �ڵ������˴ϴ�. EffectDuration�� 0�̶�� batchCount�� ���Ѹ�ŭ ��� �����˴ϴ�.")]
    [SerializeField][Range(1, 20)] private int projectileAmountInBatch = 1;
    [Tooltip("����ü�� ������ Ƚ���Դϴ�.")]
    [SerializeField][Range(1, 20)] private int batchCount = 1;
    [Tooltip("Enemy�� ��ġ�� �߽����� ����ü�� ������ �ݰ��Դϴ�.")]
    [SerializeField] private float rainRadius;
    [Tooltip("����ü�� �浹���� �� ȿ�� �ݰ��Դϴ�.")]
    [SerializeField] private float effectRadius;
    [Tooltip("����ü�� �������� �ӵ��Դϴ�.")]
    [SerializeField] private float dropSpeed;
    [Tooltip("����ü�� ���� ������ ������ �ɸ��� �ð��Դϴ�.")]
    [SerializeField] private float dropTime;
    [Tooltip("isTargeting�� true��� Target�� �߽��� �������� ����ü�� �������ϴ�.")]
    [SerializeField] private bool isTargeting;
    private Enemy enemy;
    private float createBatchDelay;
    private int currentCreatedBatch;
    private ObjectPool<Projectile> projectilePool;
    // AOE�� ǥ���� �� Y�� �������� �߰��� �����Դϴ�. effectRadius�� ���ؼ� ū ���� �����մϴ�.
    private static readonly float additionalAOEYOffset = 1.5f;

    public ProjectileRain()
    {
        ActionType = ActionTypes.ProjectileRain;
    }
    public override void OnAwake()
    {
        base.OnAwake();
        enemy = StateMachine.Enemy;
        if (batchCount == 1)
            createBatchDelay = 0;
        else
            createBatchDelay = Config.EffectDurationSeconds / (batchCount - 1);
        projectilePool = new ObjectPool<Projectile>(() => ProjectileCreateFunc(), projectile => { projectile.OnGet(); projectile.SetDisappearTime(dropTime + 1f); }, projectile => projectile.OnRelease(), defaultCapacity: projectileAmountInBatch * batchCount, maxSize: projectileAmountInBatch * batchCount * 3);
    }
    public override void OnStart()
    {
        base.OnStart();
        enemy.AnimEventReceiver.AnimEventCalled += AnimationEventDecision;
        currentCreatedBatch = 0;
        StartAnimation(Config.AnimTriggerHash1);
    }
    public override void OnEnd()
    {
        base.OnEnd();
        enemy.AnimEventReceiver.AnimEventCalled -= AnimationEventDecision;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (isEffectStarted)
        {
            if (createBatchDelay <= 0f)
            {
                while (currentCreatedBatch < batchCount)
                {
                    CreateProjectileBatch();
                }
                
            }
            else
            {
                if (currentCreatedBatch < batchCount && currentCreatedBatch < Mathf.FloorToInt((Time.time - EffectStartTime) / createBatchDelay) + 1)
                {
                    CreateProjectileBatch();
                }
            }
        }
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed && isEffectStarted)
        {
            isCompleted = true;
        }
    }
    protected override void OnEffectStart()
    {
        base.OnEffectStart();
    }
    protected override void OnEffectFinish()
    {
        base.OnEffectFinish();
    }
    private void AnimationEventDecision(AnimationEvent animEvent)
    {
        if (animEvent.stringParameter == "DropProjectile")
        {
            OnEffectStart();
        }
    }
    private Projectile ProjectileCreateFunc()
    {
        Projectile projectile = Instantiate(projectilePrefab);
        projectile.ProjetileColliderEnter += ProjectileTriggerEnter;
        projectile.TimeExpired += ProjectileTimeExpired;
        return projectile;
    }

    private void ProjectileTriggerEnter(Collider other, Projectile projectile)
    {
        projectilePool.Release(projectile);
        Collider[] colliders = Physics.OverlapSphere(projectile.transform.position, effectRadius);

        foreach (Collider collider in colliders)
        {
            ApplyAttack(collider.gameObject, isPossibleMultihit: true);
        }
    }
    private void ProjectileTimeExpired(Projectile projectile)
    {
        projectilePool.Release(projectile);
    }
    private void CreateProjectileBatch()
    {
        currentCreatedBatch++;
        Vector3 basePosition;
        if (isTargeting)
            basePosition = StateMachine.Player.transform.position;
        else
            basePosition = enemy.transform.position;
        basePosition.y += rainRadius * 1.5f;
        Vector2 randomCircle;
        Projectile projectile;
        Vector3 initialPosition;
        for (int i = 0; i < projectileAmountInBatch; i++)
        {
            randomCircle = Random.insideUnitCircle * rainRadius;
            projectile = projectilePool.Get();
            initialPosition.x = basePosition.x + randomCircle.x;
            initialPosition.y = basePosition.y;
            initialPosition.z = basePosition.z + randomCircle.y;
            Ray ray = new Ray(initialPosition, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rainRadius * 3, LayerMask.NameToLayer(TagsAndLayers.GroundLayer)))
                initialPosition.y = hit.point.y + dropSpeed * dropTime;
            else
                initialPosition.y = initialPosition.y -(rainRadius * 1.5f) + dropSpeed * dropTime;

            projectile.SetProjectileInfo(ProjectileLaunchTypes.Drop, initialPosition, Vector3.down, dropSpeed, enemy);
            projectile.IndicateCircleAOE(radius: 1, depth: dropSpeed * dropTime + Mathf.Max(effectRadius, additionalAOEYOffset));
            projectile.LerpIndicatorScale(effectRadius, dropTime);
            projectile.Launch();
        }
    }
}
