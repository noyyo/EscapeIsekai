using UnityEngine;
using UnityEngine.Pool;

public class ExplosionController : MonoBehaviour
{
    private IObjectPool<ExplosionController> managedPool;
    private int damage;
    private AttackEffectTypes attackEffectTypes;
    private float attackEffectValue;
    private GameObject attacker;
    private LayerMask layer;
    private Collider[] colliders;
    private float explosiondelayTime;
    private float explosionRadius;
    private readonly string takeDamageString = "TakeDamage";
    private ParticleSystem particleSystems;

    private void Awake()
    {
        particleSystems = GetComponent<ParticleSystem>();
    }

    public void SetManagedPool(IObjectPool<ExplosionController> pool)
    {
        managedPool = pool;
    }

    public void Explosion()
    {
        Invoke(takeDamageString, explosiondelayTime);
    }

    private void TakeDamage()
    {
        particleSystems.Play(true);
        int count = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders, layer);
        for (int i = 0; i < count; i++)
        {
            IDamageable target = null;
            
            if (colliders[i].CompareTag(TagsAndLayers.PlayerTag))
            {
                Player player;
                if (colliders[i].TryGetComponent(out player))
                {
                    target = player.StateMachine;
                }
            }
            else
            {
                Enemy enemy;
                if (colliders[i].TryGetComponent(out enemy))
                {
                    target = enemy.StateMachine;
                }
            }

            target?.TakeDamage(damage, this.gameObject);
            target?.TakeEffect(attackEffectTypes, attackEffectValue, this.attacker);
        }
    }

    public void ExplosionInit(Transform transform, Vector3 pos, float newExplosionRadius, LayerMask newlayer, int MaxCollisionObject = 20, float newExplosiondelayTime = 0)
    {
        this.transform.parent = transform;
        this.transform.position = pos;
        explosionRadius = newExplosionRadius;
        layer = newlayer;
        if (colliders == null)
        {
            colliders = new Collider[MaxCollisionObject];
        }
        else if (colliders.Length < MaxCollisionObject)
        {
            colliders = new Collider[MaxCollisionObject];
        }
        explosiondelayTime = newExplosiondelayTime;
    }

    public void ExplosionInitEnemyData(int newDamage, AttackEffectTypes newAttackEffectTypes, float newAttackEffectValue, GameObject newAttacker)
    {
        damage = newDamage;
        attackEffectTypes = newAttackEffectTypes;
        attackEffectValue = newAttackEffectValue;
        attacker = newAttacker;
    }

    private void OnDisable()
    {
        managedPool.Release(this);
    }
}
