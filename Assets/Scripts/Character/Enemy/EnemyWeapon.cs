using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public Collider WeaponCollider;
    private Enemy enemy;
    public event Action<Collider> WeaponColliderEnter;
    private static readonly string MeleeAttackStart = "MeleeAttackStart";
    private static readonly string MeleeAttackEnd = "MeleeAttackEnd";
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("Enemy객체를 찾을 수 없습니다.");
            return;
        }
        enemy.AnimationEventCalled += EventDecision;
    }
    public void EventDecision(AnimationEvent animEvent)
    {
        if (animEvent.stringParameter == MeleeAttackStart)
        {
            ActivateWeaponCollider();
        }
        else if (animEvent.stringParameter == MeleeAttackEnd)
        {
            DeactivateWeaponCollider();
        }
    }
    private void ActivateWeaponCollider()
    {
        WeaponCollider.enabled = true;
    }
    private void DeactivateWeaponCollider()
    {
        WeaponCollider.enabled = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        WeaponColliderEnter?.Invoke(other);
    }
}
