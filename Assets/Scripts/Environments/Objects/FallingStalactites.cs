using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class FallingStalactites : BaseEnvironmentObject
{
    [SerializeField] private AttackEffectTypes attackEffectType;
    [SerializeField] private float attackEffectValue;
    [Tooltip("해당 시간이 지날 때까지 부딪히지 않는다면 자동으로 소멸됩니다.")]
    [SerializeField][Range(1f, 20f)] private float disappearTime = 5f;
    [Tooltip("투사체가 충돌했을 때 효과 반경입니다.")]
    [SerializeField] private float effectRadius;
    private FallingStalactitesManager stalactitesManager;
    private IObjectPool<FallingStalactites> managedPool;
    private AOEIndicator aoeIndicator;
    private Collider collider;
    private Vector3 colliderSize;
    private Transform thisTransform;
    private float fallingSpeed;
    private WaitForSeconds delayTime;
    private FallingStalactitesTarget target;
    private float posY;
    private int damage = 5;
    private bool isTrue;

    private void Awake()
    {
        thisTransform = this.transform;
        collider = GetComponent<Collider>();
        colliderSize = collider.bounds.extents * 2;
        stalactitesManager = FallingStalactitesManager.Instance;
    }

    public void SetManagedPool(IObjectPool<FallingStalactites> pool)
    {
        managedPool = pool;
    }

    public override void TakeDamage(int damage)
    { }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(TagsAndLayers.GroundLayer))
        {
            StopCoroutine(DownPosition());
            stalactitesManager.OnRelease(aoeIndicator);
            PlayAnimationFadeOut();
            return;
        }

        IDamageable targetObject = null;
        Enemy enemy;
        Player player;
        switch (target)
        {
            case FallingStalactitesTarget.All:
                {
                    if (other.CompareTag(TagsAndLayers.PlayerTag))
                    {
                        isTrue = true;
                        if (!other.TryGetComponent<Player>(out player))
                            player = other.GetComponentInParent<Player>();
                        targetObject = player.StateMachine;
                    }
                    else if (other.CompareTag(TagsAndLayers.EnemyTag))
                    {
                        isTrue = true;
                        if (!other.TryGetComponent<Enemy>(out enemy))
                            enemy = other.GetComponentInParent<Enemy>();
                        targetObject = enemy.StateMachine;
                    }
                    break;
                }
            case FallingStalactitesTarget.Player:
                if (other.CompareTag(TagsAndLayers.PlayerTag))
                {
                    isTrue = true;
                    if (!other.TryGetComponent<Player>(out player))
                        player = other.GetComponentInParent<Player>();
                    targetObject = player.StateMachine;
                }
                break;
            case FallingStalactitesTarget.Enemy:
                if (other.CompareTag(TagsAndLayers.EnemyTag))
                {
                    isTrue = true;
                    if (!other.TryGetComponent<Enemy>(out enemy))
                        enemy = other.GetComponentInParent<Enemy>();
                    targetObject = enemy.StateMachine;
                }
                break;
        }
        if (isTrue)
        {
            targetObject?.TakeDamage(damage);
            targetObject?.TakeEffect(attackEffectType, attackEffectValue, this.gameObject);
            isTrue = false;
            PlayAnimationFadeOut();
            stalactitesManager.OnRelease(aoeIndicator);
            aoeIndicator = null;
        }
    }

    public void Falling(Transform transform, Vector3 vector3, int newDamage, float speed, float fallStartTime, FallingStalactitesTarget attackTarget, float limitPosY = 0, float radius = 0, float depth = 0)
    {
        thisTransform.parent = transform;
        thisTransform.position = vector3;
        damage = newDamage;
        fallingSpeed = speed;
        target = attackTarget;
        posY = limitPosY;
        delayTime = new WaitForSeconds(fallStartTime);


        if (radius <= 0)
            radius = Mathf.Max(colliderSize.x, colliderSize.y, colliderSize.z);

        if (depth <= 0)
            depth = speed * disappearTime;

        aoeIndicator.IndicateCircleAOE(vector3, Vector3.down, radius, depth, false);
        aoeIndicator.LerpIndicatorScale(2, 5); // 효과 범위 및 도착까지 걸리는 시간

        StartCoroutine(DownPosition());
    }

    private IEnumerator DownPosition()
    {
        //yield return delayTime;
        while (true)
        {
            transform.Translate(Vector3.down * Time.deltaTime * fallingSpeed);
            if (transform.position.y <= posY)
                yield break;
            yield return null;
        }
    }
    private void PlayAnimationFadeOut()
    {
        //임시 코드
        Destroy();
    }

    private void Destroy()
    {
        managedPool.Release(this);
    }

    public void SetAOEIndicator(AOEIndicator newAOEIndicator)
    {
        aoeIndicator = newAOEIndicator;
    }

    public override Vector3 GetObjectCenterPosition()
    {
        throw new System.NotImplementedException();
    }
}
