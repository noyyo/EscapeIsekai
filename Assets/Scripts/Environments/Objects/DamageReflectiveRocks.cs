using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DamageReflectiveRocks : BaseEnvironmentObject
{

    [Tooltip("대미지 배율 - 기본값 1")][SerializeField] private float _magnification = 1;

    [Tooltip("내가 원하는 값으로 커스텀 - 위의 배율도 적용됨")][Header("Csutom")]
    [SerializeField] bool _isCustomValue;
    [SerializeField] private int _customDamage;
    [SerializeField] private float _customValue;
    [SerializeField] private AttackEffectTypes _customAttackEffectTypes;
    
    private int _damage;
    private float _value;
    private AttackEffectTypes _attackEffectTypes;

    private void Start()
    {
        _damage = (int)(_customDamage * _magnification);
        _value = _customValue;
        _attackEffectTypes = _customAttackEffectTypes;
    }

    public override void TakeDamage(int damage)
    {
        if (!_isCustomValue)
            _damage = (int)(damage * _magnification);
    }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    {
        if (!_isCustomValue)
        {
            _value = value;
            _attackEffectTypes = attackEffectTypes;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = null;
        if (other.tag == Tags.EnemyTag)
        {
            Enemy enemy;
            enemy = other.GetComponentInParent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError("적에게 Enemy컴포넌트가 없습니다.");
                return;
            }
            target = enemy.StateMachine;
        }
        target?.TakeDamage(_damage);
        target?.TakeEffect(_attackEffectTypes, _value, this.gameObject);
    }
}

