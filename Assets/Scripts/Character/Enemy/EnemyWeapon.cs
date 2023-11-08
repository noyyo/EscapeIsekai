using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public Collider WeaponCollider;
    public Enemy enemy;
    public event Action<Collision> WeaponCollisionEnter;
    private void Awake()
    {
        enemy.AnimationEventCalled += ActivateWeaponCollider;
    }
    public void EventDecision(AnimationEvent animEvent)
    {
        if (animEvent.functionName == "Action1")
        {
            ActivateWeaponCollider();
        }
        WeaponCollider.enabled = true;
    }
    public void DeactivateWeaponCollider()
    {
        WeaponCollider.enabled = false;
    }
    public void OnCollisionEnter(Collision collision)
    {
        WeaponCollisionEnter?.Invoke(collision);
    }
}
