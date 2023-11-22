using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public enum BulletType
{
    Nomal,
    ShotGun
}
[CreateAssetMenu(fileName = "TestAction_ProjectileSO", menuName = "Characters/Enemy/AttackAction/TestAction_Projectile")]
public class TestAction_Projectile : AttackAction
{
    [Header("����ü ����")]
    [Tooltip(nameof(Projectile) + "�� ������ �ִ� Prefab")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField][Range(1, 36)] private int projectileAmount;
    [SerializeField][Range(0.5f, 30f)] private float projectileSpeed;
    [Tooltip("����ü�� ����������� �߻�Ǵ� �����Դϴ�.")]
    [SerializeField][Range(0f, 360f)] private float horizontalAngle;
    [Tooltip("����ü�� ������������ �߻�Ǵ� ����. ����ü���� ������������ �� ������ŭ ȸ���Ǿ� �߻�˴ϴ�. isTargeting �ɼ��� ���� ��� ���õ˴ϴ�.")]
    [SerializeField][Range(-90f, 90f)] private float verticalAngle;
    [Tooltip("����ü���� �Ÿ��Դϴ�. ���򰢵��� 0�� ��� �Ϸķ� spacing�� ������ �ΰ� ��ġ�˴ϴ�. 0�� �ƴ� ��� ���õ˴ϴ�.")]
    [SerializeField][Range(0f, 1f)] private float spacing;
    [Tooltip("isTargeting �ɼ��� �����ִٸ� Ÿ���� ������ verticalAngle�� �����մϴ�.")]
    [SerializeField] private bool isTargeting;
    [Tooltip("����ü�� Enemy�� Transform���� ������ ������.")]
    [SerializeField] private Vector3 offset;

    private ObjectPool<Projectile> projectilePool;
    private List<Projectile> settedProjectiles;
    public override void OnAwake()
    {
        base.OnAwake();
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled += EventDecision;
        PoolInitialize();
    }
    private void PoolInitialize()
    {
        projectilePool = new ObjectPool<Projectile>(() => ProjectileCreateFunc(), projectile => projectile.gameObject.SetActive(true), projectile => { projectile.gameObject.SetActive(false); projectile.rigidbody.velocity = Vector3.zero; }, defaultCapacity: projectileAmount, maxSize: projectileAmount * 3);
        settedProjectiles = new List<Projectile>(projectileAmount);
    }
    private Projectile ProjectileCreateFunc()
    {
        Projectile projectile = Instantiate(projectilePrefab);
        projectile.ProjetileColliderEnter += OnProjectileTriggerEnter;
        projectile.TimeExpired += OnDisappearTimeExpired;
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
            LaunchProjectile();
        }
    }
    public override void OnStart()
    {
        base.OnStart();
        StartAnimation(Config.AnimTriggerHash1);
    }
    public override void OnEnd()
    {
        base.OnEnd();
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
    private void OnProjectileTriggerEnter(GameObject target, Projectile projectile)
    {
        if (target.layer == LayerMask.NameToLayer(TagsAndLayers.GroundLayer))
        {
            projectilePool.Release(projectile);
            return;
        }
        ApplyAttack(target);

    }
    private void OnDisappearTimeExpired(Projectile projectile)
    {
        projectilePool.Release(projectile);
    }
    private void SetProjectile()
    {
        if (isTargeting)
        {
            Vector3 verticalDirection = StateMachine.Player.transform.position - StateMachine.Enemy.transform.position;
            verticalDirection.Normalize();
            verticalDirection.x = 0;
            verticalDirection.z = 0;
            verticalAngle = Mathf.Asin(verticalDirection.y) * Mathf.Rad2Deg;
        }

        if (horizontalAngle == 0f)
            SetProjectileInStraight();
        else
            SetProjectileInCircle();
    }
    private void SetProjectileInStraight()
    {
        Vector3 startPosition = StateMachine.Enemy.transform.TransformPoint(offset + new Vector3((projectileAmount - 1) * -spacing / 2, 0, 0));
        Quaternion verticalRotation = Quaternion.Euler(verticalAngle, 0, 0);
        Vector3 direction = StateMachine.Enemy.transform.TransformDirection(verticalRotation * Vector3.forward);
        Vector3 nextProjectilePosition = startPosition;

        for (int i = 0; i < projectileAmount; i++)
        {
            Projectile projectile = projectilePool.Get();
            projectile.SetProjectile(nextProjectilePosition, direction, projectileSpeed, StateMachine.Enemy, 1f);
            settedProjectiles.Add(projectile);
            nextProjectilePosition = projectile.transform.TransformPoint(new Vector3(spacing, 0, 0));
        }
    }
    private void SetProjectileInCircle()
    {
        Quaternion horizontalRotation = Quaternion.Euler(0, -horizontalAngle / projectileAmount, 0);
        Quaternion verticalRotation = Quaternion.Euler(verticalAngle, 0, 0);
        Vector3 rotatedOffset;
        Vector3 startPosition;
        Quaternion startRotation;
        if (projectileAmount % 2 == 0)
        {
            startRotation = Quaternion.Euler(0, horizontalAngle / 2, 0) * Quaternion.Euler(0, -(horizontalAngle / projectileAmount) / 2, 0);
            rotatedOffset = startRotation * offset;
            startPosition = StateMachine.Enemy.transform.TransformPoint(rotatedOffset);
        }
        else
        {
            startRotation = Quaternion.Euler(0, horizontalAngle / 2, 0);
            rotatedOffset = startRotation * offset;
            startPosition = StateMachine.Enemy.transform.TransformPoint(rotatedOffset);
        }
        Vector3 nextProjectilePosition = startPosition;
        Quaternion nextProjectileRotation = startRotation;
        for (int i = 0; i < projectileAmount; i++)
        {
            Projectile projectile = projectilePool.Get();
            Vector3 direction = StateMachine.Enemy.transform.TransformDirection(nextProjectileRotation * verticalRotation * Vector3.forward);
            projectile.SetProjectile(nextProjectilePosition, direction, projectileSpeed, StateMachine.Enemy, 1f);
            settedProjectiles.Add(projectile);
            nextProjectileRotation = nextProjectileRotation * horizontalRotation;
            rotatedOffset = nextProjectileRotation * offset;
            nextProjectilePosition = StateMachine.Enemy.transform.TransformPoint(rotatedOffset);
        }
    }
    private void LaunchProjectile()
    {
        for (int i = settedProjectiles.Count - 1; i >= 0; i--)
        {
            settedProjectiles[i].Launch();
            settedProjectiles.RemoveAt(i);
        }
    }
    
}