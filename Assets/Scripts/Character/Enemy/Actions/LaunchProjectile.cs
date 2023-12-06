using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


[CreateAssetMenu(fileName = "LaunchProjectile", menuName = "Characters/Enemy/AttackAction/LaunchProjectile")]
public class LaunchProjectile : AttackAction
{
    [Header("����ü ����")]
    [Tooltip(nameof(Projectile) + "�� ������ �ִ� Prefab")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField][Range(1, 36)] private int projectileAmount = 1;
    [SerializeField][Range(0.5f, 30f)] private float projectileSpeed;
    [Tooltip("����ü�� ����������� �߻�Ǵ� �����Դϴ�.")]
    [SerializeField][Range(0f, 360f)] private float horizontalAngle;
    [Tooltip("����ü�� ������������ �߻�Ǵ� ����. ����ü���� ������������ �� ������ŭ ȸ���Ǿ� �߻�˴ϴ�. isTargeting �ɼ��� ���� ��� ���õ˴ϴ�.")]
    [SerializeField][Range(-60f, 60f)] private float verticalAngle;
    [Tooltip("����ü���� �Ÿ��Դϴ�. ���򰢵��� 0�� ��� �Ϸķ� spacing�� ������ �ΰ� ��ġ�˴ϴ�. 0�� �ƴ� ��� ���õ˴ϴ�.")]
    [SerializeField][Range(0f, 5f)] private float spacing;
    [Tooltip("isTargeting �ɼ��� �����ִٸ� Ÿ���� ������ verticalAngle�� �����մϴ�.")]
    [SerializeField] private bool isTargeting;
    [Tooltip("����ü �������� �������� ��ġ��ŭ ���մϴ�.")]
    [SerializeField] private float sizeScale = 1f;
    [Tooltip("����ü�� PointReference�� Transform���� ������ ������.")]
    [SerializeField] private Vector3 offset;

    private PointReference LaunchPointReference;
    private ObjectPool<Projectile> projectilePool;
    private List<Projectile> settedProjectiles;
    private Transform enemyTransform;
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
    /// ����ü�� Ʈ���ſ� �������� ������ �޾� ����˴ϴ�.
    /// </summary>
    /// <param name="target">Ʈ���ſ� ���� ��ü�Դϴ�.</param>
    /// <param name="projectile">����ü �ڽ��� �����Դϴ�.</param>
    private void OnProjectileTriggerEnter(Collider other, Projectile projectile)
    {
        if (other.gameObject.layer == TagsAndLayers.GroundLayerIndex)
        {
            projectilePool.Release(projectile);
        }
        else if (other.gameObject.layer == TagsAndLayers.CharacterLayerIndex || other.CompareTag(TagsAndLayers.EnvironmentTag))
        {
            GameObject target = other.gameObject;
            ApplyAttack(target, true);
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

}