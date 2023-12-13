using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.EventSystems.EventTrigger;


[CreateAssetMenu(fileName = "LaunchProjectile", menuName = "Characters/Enemy/AttackAction/LaunchProjectile")]
public class LaunchProjectile : AttackAction
{
    [Header("투사체 설정")]
    [Tooltip(nameof(Projectile) + "을 가지고 있는 Prefab")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField][Range(1, 36)] private int projectileAmount = 1;
    [SerializeField][Range(0.5f, 30f)] private float projectileSpeed;
    [Tooltip("투사체가 수평방향으로 발사되는 각도입니다.")]
    [SerializeField][Range(0f, 360f)] private float horizontalAngle;
    [Tooltip("투사체가 수직방향으로 발사되는 각도. 투사체들은 수직방향으로 이 각도만큼 회전되어 발사됩니다. isTargeting 옵션이 켜진 경우 무시됩니다.")]
    [SerializeField][Range(-60f, 60f)] private float verticalAngle;
    [Tooltip("투사체간의 거리입니다. 수평각도가 0인 경우 일렬로 spacing의 간격을 두고 배치됩니다. 0이 아닐 경우 무시됩니다.")]
    [SerializeField][Range(0f, 5f)] private float spacing;
    [Tooltip("isTargeting 옵션이 켜져있다면 타겟을 조준해 verticalAngle을 조정합니다.")]
    [SerializeField] private bool isTargeting;
    [Tooltip("투사체 프리팹의 스케일을 수치만큼 곱합니다.")]
    [SerializeField] private float sizeScale = 1f;
    [Tooltip("투사체가 PointReference의 Transform에서 가지는 변위차.")]
    [SerializeField] private Vector3 offset;

    private PointReference LaunchPointReference;
    private ObjectPool<Projectile> projectilePool;
    private List<Projectile> settedProjectiles;
    private Transform enemyTransform;
    private HashSet<Projectile> hitProjectiles = new HashSet<Projectile>(20);

    public LaunchProjectile()
    {
        ActionType = ActionTypes.LaunchProjectile;
    }
    public override void OnAwake()
    {
        base.OnAwake();
        LaunchPointReference = StateMachine.Enemy.PointReferences[PointReferenceTypes.LaunchProjectile];
        enemyTransform = StateMachine.Enemy.transform;
        PoolInitialize();
    }
    private void PoolInitialize()
    {
        projectilePool = new ObjectPool<Projectile>(() => ProjectileCreateFunc(), projectile => projectile.OnGet(), projectile => projectile.OnRelease(), defaultCapacity: projectileAmount, maxSize: projectileAmount * 3);
        settedProjectiles = new List<Projectile>(projectileAmount);
    }
    private Projectile ProjectileCreateFunc()
    {
        Projectile projectile = Instantiate(projectilePrefab);
        projectile.transform.localScale = projectile.transform.localScale * sizeScale;
        projectile.ProjetileColliderEnter += OnProjectileTriggerEnter;
        projectile.TimeExpired += OnDisappearTimeExpired;
        projectile.gameObject.SetActive(false);
        return projectile;
    }

    private void EventDecision(AnimationEvent animEvent)
    {
        if (animEvent.stringParameter == "SetProjectile")
        {
            SetProjectile();
        }
        else if (animEvent.stringParameter == "LaunchProjectile")
        {
            LaunchProjectiles();
        }
    }
    public override void OnStart()
    {
        base.OnStart();
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled += EventDecision;
        StartAnimation(Config.AnimTriggerHash1);
    }
    public override void OnEnd()
    {
        base.OnEnd();
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled -= EventDecision;
        hitProjectiles.Clear();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed)
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
    /// <summary>
    /// 투사체의 트리거에 들어왔을때 정보를 받아 실행됩니다.
    /// </summary>
    /// <param name="target">트리거에 들어온 객체입니다.</param>
    /// <param name="projectile">투사체 자신의 정보입니다.</param>
    private void OnProjectileTriggerEnter(Collider other, Projectile projectile)
    {
        if (other.gameObject.layer == TagsAndLayers.GroundLayerIndex)
        {
            projectilePool.Release(projectile);
        }
        else if (other.gameObject.layer == TagsAndLayers.PlayerLayerIndex || other.gameObject.layer == TagsAndLayers.CharacterLayerIndex || other.CompareTag(TagsAndLayers.EnvironmentTag))
        {
            if (!hitProjectiles.Contains(projectile))
            {
                hitProjectiles.Add(projectile);
                if (other.gameObject == other.gameObject)
                ApplyAttack(other.gameObject, isPossibleMultihit: true);
            }
            projectilePool.Release(projectile);
        }
    }
    private void OnDisappearTimeExpired(Projectile projectile)
    {
        projectilePool.Release(projectile);
    }
    private void SetProjectile()
    {
        if (isTargeting)
        {
            IPositionable target = StateMachine.PositionableTarget;
            Vector3 verticalDirection = target.GetObjectCenterPosition() - (LaunchPointReference.transform.position + offset);
            verticalDirection.Normalize();
            verticalDirection.x = 0;
            verticalDirection.z = 0;
            verticalAngle = -Mathf.Asin(verticalDirection.y) * Mathf.Rad2Deg;
            Mathf.Clamp(verticalAngle, -60f, 60f);
        }

        if (horizontalAngle == 0f)
            SetProjectileInStraight();
        else
            SetProjectileInCircle();
    }
    private void SetProjectileInStraight()
    {
        Vector3 LaunchPointRelativePosition = enemyTransform.InverseTransformPoint(LaunchPointReference.transform.position) + offset;
        Vector3 startPosition = enemyTransform.TransformPoint(LaunchPointRelativePosition + new Vector3((projectileAmount - 1) / 2f * -spacing, 0, 0));
        Quaternion verticalRotation = Quaternion.Euler(verticalAngle, 0, 0);
        Vector3 direction = enemyTransform.TransformDirection(verticalRotation * Vector3.forward);
        Vector3 nextProjectilePosition = startPosition;
        for (int i = 0; i < projectileAmount; i++)
        {
            Projectile projectile = projectilePool.Get();
            projectile.SetProjectileInfo(ProjectileLaunchTypes.Shoot, nextProjectilePosition, direction, projectileSpeed, StateMachine.Enemy);
            projectile.IndicateBoxAOE(yOffset: 1.5f);
            settedProjectiles.Add(projectile);
            nextProjectilePosition = projectile.transform.TransformPoint(new Vector3(spacing, 0, 0) / projectile.transform.localScale.x);
        }
    }
    private void SetProjectileInCircle()
    {
        Vector3 LaunchPointRelativePosition = enemyTransform.InverseTransformPoint(LaunchPointReference.transform.position);
        Quaternion horizontalRotation = Quaternion.Euler(0, -horizontalAngle / projectileAmount, 0);
        Quaternion verticalRotation = Quaternion.Euler(verticalAngle, 0, 0);
        Vector3 rotatedOffset;
        Vector3 startPosition;
        Quaternion startRotation;
        if (projectileAmount % 2 == 0)
        {
            startRotation = Quaternion.Euler(0, horizontalAngle / 2, 0) * Quaternion.Euler(0, -(horizontalAngle / projectileAmount) / 2, 0);
            rotatedOffset = startRotation * offset;
            startPosition = enemyTransform.TransformPoint(LaunchPointRelativePosition + rotatedOffset);
        }
        else
        {
            startRotation = Quaternion.Euler(0, horizontalAngle / 2, 0);
            rotatedOffset = startRotation * offset;
            startPosition = enemyTransform.TransformPoint(LaunchPointRelativePosition + rotatedOffset);
        }

        Vector3 nextProjectilePosition = startPosition;
        Quaternion nextProjectileRotation = startRotation;
        for (int i = 0; i < projectileAmount; i++)
        {
            Projectile projectile = projectilePool.Get();
            Vector3 direction = enemyTransform.TransformDirection(nextProjectileRotation * verticalRotation * Vector3.forward);
            projectile.SetProjectileInfo(ProjectileLaunchTypes.Shoot, nextProjectilePosition, direction, projectileSpeed, StateMachine.Enemy);
            projectile.IndicateBoxAOE(yOffset: 1.5f);
            settedProjectiles.Add(projectile);
            nextProjectileRotation = nextProjectileRotation * horizontalRotation;
            rotatedOffset = nextProjectileRotation * offset;
            nextProjectilePosition = enemyTransform.TransformPoint(LaunchPointRelativePosition + rotatedOffset);
        }
    }
    private void LaunchProjectiles()
    {
        for (int i = settedProjectiles.Count - 1; i >= 0; i--)
        {
            settedProjectiles[i].Launch();
            settedProjectiles[i].ReleaseIndicator();
            settedProjectiles.RemoveAt(i);
        }
    }
    protected override void ReleaseIndicator()
    {
    }
}