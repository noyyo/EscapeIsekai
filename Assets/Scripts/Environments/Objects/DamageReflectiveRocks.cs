using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DamageReflectiveRocks : BaseEnvironmentObject
{
    [SerializeField] private int _damage;
    [SerializeField] private float _value;
    [SerializeField] private GameObject _attacker;
    public override void TakeDamage(int damage)
    {
        _damage = damage;
    }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    {
        _value = value;
        _attacker = attacker;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = null;
        if (other.tag == Tags.EnemyTag)
        {
            Enemy enemy;
            other.TryGetComponent(out enemy);
            if (enemy == null)
            {
                Debug.LogError("적에게 Enemy컴포넌트가 없습니다.");
                return;
            }
            target = enemy.StateMachine;
        }
        target?.TakeDamage(_damage);
        target?.TakeEffect(AttackEffectTypes.Stun, _value/2, _attacker);
    }
}

