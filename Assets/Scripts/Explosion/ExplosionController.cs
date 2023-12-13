using System.Collections.Generic;
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
    private readonly string AttackString = "Attack";
    private ParticleSystem particleSystems;
    private bool isAttackedPlayer;
    private Queue<Enemy> enemyQueue;
    private Enemy attackerEnemy;

    private void Awake()
    {
        particleSystems = GetComponent<ParticleSystem>();
        enemyQueue = new Queue<Enemy>();
    }

    public void SetManagedPool(IObjectPool<ExplosionController> pool)
    {
        managedPool = pool;
    }

    public void Explosion()
    {
        Invoke(AttackString, explosiondelayTime);
    }

    private void Attack()
    {
        attackerEnemy = attacker.GetComponent<Enemy>();
        particleSystems.Play(true);
        isAttackedPlayer = false;
        int count = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders, layer);
        Debug.Log(count);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].CompareTag(TagsAndLayers.PlayerTag) && !isAttackedPlayer)
            {
                IDamageable target = null;
                Player player;
                isAttackedPlayer = true;
                if (colliders[i].TryGetComponent(out player))
                {
                    target = player.StateMachine;
                    target.TakeDamage(damage, attacker);
                    target.TakeEffect(attackEffectTypes, attackEffectValue, attacker);
                } 
            }
            else
            {
                Enemy enemy;
                if (colliders[i].TryGetComponent(out enemy))
                {
                    if (!enemyQueue.Contains(enemy) && enemy != attackerEnemy)
                        enemyQueue.Enqueue(enemy);
                }
            }
            
        }

        foreach(Enemy attackedEnemy in enemyQueue)
        {
            IDamageable enemyTarget = attackedEnemy.StateMachine;
            enemyTarget?.TakeDamage(damage, attacker);
            enemyTarget?.TakeEffect(attackEffectTypes, attackEffectValue, attacker);
        }
    }

    public void ExplosionInit(Transform newTransform, Vector3 pos, float newExplosionRadius, LayerMask newlayer, int MaxCollisionObject = 20, float newExplosiondelayTime = 0)
    {
        transform.parent = newTransform;
        transform.position = pos;
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
