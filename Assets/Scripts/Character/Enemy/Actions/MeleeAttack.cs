using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Characters/Enemy/AttackAction/MeleeAttack")]
public class MeleeAttack : AttackAction
{
    [ReadOnly] public EnemyWeapon Weapon;

    public override void OnAwake()
    {
        base.OnAwake();
        Weapon = StateMachine.Enemy.GetComponentInChildren<EnemyWeapon>();
        if (Weapon == null)
            Debug.LogError("Weapon이 필요합니다.");
        Weapon.WeaponColliderEnter += OnWeaponTriggerEnter;
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
    private void OnWeaponTriggerEnter(Collider other)
    {
        if (StateMachine.Player.gameObject == other.gameObject)
        {
            Player target = other.gameObject.GetComponent<Player>();
            IDamageable damageableTarget = target as IDamageable;
            ApplyAttack(damageableTarget);
        }
    }
}
