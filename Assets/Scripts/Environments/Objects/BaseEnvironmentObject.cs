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
    [SerializeField] protected List<CanTakeDamageCharacterTypes> canTakeDamageCharacterTypes = new List<CanTakeDamageCharacterTypes>();
    public AffectedAttackEffectInfo AffectedEffectInfo { get => affectedEffectInfo; }

    public event Action OnDie;

    public abstract Vector3 GetObjectCenterPosition();

    public abstract void TakeDamage(int damage, GameObject attacker);
    public abstract void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker);
    protected bool CanTakeDamage(GameObject attacker)
    {
        CanTakeDamageCharacterTypes type = CanTakeDamageCharacterTypes.None;
        if (attacker.CompareTag(TagsAndLayers.PlayerTag))
            type = CanTakeDamageCharacterTypes.Player;
        else if (attacker.CompareTag(TagsAndLayers.EnemyTag))
            type = CanTakeDamageCharacterTypes.Enemy;
        else if (attacker.CompareTag(TagsAndLayers.EnvironmentTag))
            type = CanTakeDamageCharacterTypes.Environment;

        foreach (var characterType in canTakeDamageCharacterTypes)
        {
            if (characterType == type)
                return true;
        }
        return false;
    }
}
