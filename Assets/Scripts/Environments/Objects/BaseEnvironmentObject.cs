using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnvironmentObject : MonoBehaviour, IDamageable
{
    protected AffectedAttackEffectInfo affectedEffectInfo;
    public AffectedAttackEffectInfo AffectedEffectInfo { get => affectedEffectInfo; }

    public event Action OnDie;
    public abstract void TakeDamage(int damage);
    public abstract void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker);
}
