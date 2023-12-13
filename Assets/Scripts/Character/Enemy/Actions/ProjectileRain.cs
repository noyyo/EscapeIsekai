using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "ProjectileRain", menuName = "Characters/Enemy/AttackAction/ProjectileRain")]
public class ProjectileRain : AttackAction
{
    [SerializeField] Projectile projectilePrefab;
    [Tooltip("한 번에 생성될 투사체의 개수입니다. 생성되는 주기는 EffectDuration에 따라 자동결정됩니다. EffectDuration이 0이라면 batchCount를 곱한만큼 즉시 생성됩니다.")]
    [SerializeField][Range(1, 20)] private int projectileAmountInBatch = 1;
    [Tooltip("투사체가 생성될 횟수입니다.")]
    [SerializeField][Range(1, 20)] private int batchCount = 1;
    [Tooltip("Enemy의 위치를 중심으로 투사체가 떨어질 반경입니다.")]
    [SerializeField] private float rainRadius;
    [Tooltip("투사체가 충돌했을 때 효과 반경입니다.")]
    [SerializeField] private float effectRadius;
    [Tooltip("투사체가 떨어지는 속도입니다.")]
    [SerializeField] private float dropSpeed;
    [Tooltip("투사체가 땅에 떨어질 때까지 걸리는 시간입니다.")]
    [SerializeField] private float dropTime;
    [Tooltip("isTargeting이 true라면 Target의 중심을 기준으로 투사체가 떨어집니다.")]
    [SerializeField] private bool isTargeting;
    private Enemy enemy;
    private float createBatchDelay;
    private int currentCreatedBatch;
    private ObjectPool<Projectile> projectilePool;
    // AOE를 표시할 때 Y축 방향으로 추가할 범위입니다. effectRadius와 비교해서 큰 값을 적용합니다.
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
        if (other.gameObject.layer == TagsAndLayers.GroundLayerIndex || other.gameObject.layer == TagsAndLayers.CharacterLayerIndex || other.gameObject.layer == TagsAndLayers.PlayerLayerIndex)
        {
            Collider[] colliders = Physics.OverlapSphere(projectile.transform.position, effectRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject == enemy.gameObject)
                    continue;
                ApplyAttack(collider.gameObject, isPossibleMultihit: true);
            }
            projectilePool.Release(projectile);
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
            if (Physics.Raycast(ray, out hit, rainRadius * 3, 1 << TagsAndLayers.GroundLayerIndex))
            {
                initialPosition.y = hit.point.y + dropSpeed * dropTime;
            }
            else
            {
                initialPosition.y = initialPosition.y - (rainRadius * 1.5f) + dropSpeed * dropTime;
            }

            projectile.SetProjectileInfo(ProjectileLaunchTypes.Drop, initialPosition, Vector3.down, dropSpeed, enemy);
            projectile.IndicateCircleAOE(radius: 1, depth: dropSpeed * dropTime + Mathf.Max(effectRadius, additionalAOEYOffset));
            projectile.LerpIndicatorScale(effectRadius, dropTime);
            projectile.Launch();
        }
    }
    protected override void ReleaseIndicator()
    {

    }
}
