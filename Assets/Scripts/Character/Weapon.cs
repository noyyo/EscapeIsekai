using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Weapon : MonoBehaviour
{
    public Collider WeaponCollider;
    [Tooltip("이 무기를 사용하는 캐릭터의 게임오브젝트를 넣어주어야 합니다.")]
    public GameObject character;
    public event Action<Collider> WeaponColliderEnter;
    private static readonly string MeleeAttackStart = "MeleeAttackStart";
    private static readonly string MeleeAttackEnd = "MeleeAttackEnd";

    private void Awake()
    {
        if (character == null)
            Debug.LogError("무기를 사용할 캐릭터가 없습니다.");
        if (character.tag == Tags.PlayerTag)
        {
            Player player = GetComponentInParent<Player>();
            if (player == null)
            {
                Debug.LogError("Player객체를 찾을 수 없습니다.");
                return;
            }
            player.AnimationEventReceiver.AnimEventCalled += EventDecision;
        }
        else if (character.tag == Tags.EnemyTag)
        {
            Enemy enemy = GetComponentInParent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError("Enemy객체를 찾을 수 없습니다.");
                return;
            }
            enemy.AnimationEventCalled += EventDecision;
        }
        else
        {
            Debug.LogError("잘못된 캐릭터입니다.");
        }

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
