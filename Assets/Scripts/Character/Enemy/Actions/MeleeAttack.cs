using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Characters/Enemy/AttackAction/MeleeAttack")]
public class MeleeAttack : AttackAction
{
    [ReadOnly] public Weapon Weapon;

    public override void OnAwake()
    {
        base.OnAwake();
        Weapon = StateMachine.Enemy.GetComponentInChildren<Weapon>();
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
        IDamageable target = null;
        if (other.tag == Tags.PlayerTag)
        {
            Player player;
            other.TryGetComponent(out player);
            if (player == null)
            {
                Debug.LogError("Player 스크립트를 찾을 수 없습니다.");
                return;
            }
            target = player.StateMachine;
        }
        else if(other.tag == Tags.EnemyTag)
        {
            Enemy enemy;
            other.TryGetComponent(out enemy);
            if (enemy == null)
            {
                Debug.LogError("Enemy 스크립트를 찾을 수 없습니다.");
                return;
            }
            target = enemy.StateMachine;
        }
        else if(other.tag == Tags.EnvironmentTag)
        {
            BaseEnvironmentObject environmentObj;
            other.TryGetComponent(out environmentObj);
            if (environmentObj == null)
            {
                Debug.LogError("대상에게 BaseEnvironmentObject 컴포넌트가 없습니다.");
                return;
            }
            target = environmentObj;
        }
        if (target != null)
        {
            ApplyAttack(target, false, other.gameObject);
        }
    }
}
