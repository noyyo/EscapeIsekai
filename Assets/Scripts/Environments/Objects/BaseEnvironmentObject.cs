using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class BaseEnvironmentObject : MonoBehaviour, IDamageable, IPositionable
{
    [SerializeField] private int hp = 100;
    public int HP
    {
        get { return hp; }
        protected set
        {
            hp = value;
            if (hp <= 0)
            {
                hp = 0;
                OnDie?.Invoke();
            }
        }
    }
    protected AffectedAttackEffectInfo affectedEffectInfo;
    [SerializeField] protected List<CanBeAttackedTypes> canBeAttackedType = new List<CanBeAttackedTypes>();
    public AffectedAttackEffectInfo AffectedEffectInfo { get => affectedEffectInfo; }

    public event Action OnDie;

    public abstract Vector3 GetObjectCenterPosition();

    public abstract void TakeDamage(int damage, GameObject attacker);
    public abstract void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker);
    protected bool CanTakeDamageAndEffect(GameObject attacker)
    {
        CanBeAttackedTypes type = CanBeAttackedTypes.None;
        if (attacker.CompareTag(TagsAndLayers.PlayerTag))
            type = CanBeAttackedTypes.Player;
        else if (attacker.CompareTag(TagsAndLayers.EnemyTag))
            type = CanBeAttackedTypes.Enemy;
        else if (attacker.CompareTag(TagsAndLayers.EnvironmentTag))
            type = CanBeAttackedTypes.Environment;

        foreach (var characterType in canBeAttackedType)
        {
            if (characterType == type)
                return true;
        }
        return false;
    }
}
