using System;
using UnityEngine;

public interface IDamageable
{
    public event Action OnDie;
    public abstract void TakeDamage(int damage, GameObject attacker);
    public AffectedAttackEffectInfo AffectedEffectInfo { get; }
    public abstract void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker);
}
