using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Weapon : MonoBehaviour
{
    public Collider WeaponCollider;
    [Tooltip("�� ���⸦ ����ϴ� ĳ������ ���ӿ�����Ʈ�� �־��־�� �մϴ�.")]
    public GameObject character;
    public event Action<Collider> WeaponColliderEnter;
    private static readonly string MeleeAttackStart = "MeleeAttackStart";
    private static readonly string MeleeAttackEnd = "MeleeAttackEnd";

    private void Awake()
    {
        if (character == null)
            Debug.LogError("���⸦ ����� ĳ���Ͱ� �����ϴ�.");
        if (character.tag == Tags.PlayerTag)
        {
            Player player = GetComponentInParent<Player>();
            if (player == null)
            {
                Debug.LogError("Player��ü�� ã�� �� �����ϴ�.");
                return;
            }
            player.AnimationEventReceiver.AnimEventCalled += EventDecision;
        }
        else if (character.tag == Tags.EnemyTag)
        {
            Enemy enemy = GetComponentInParent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError("Enemy��ü�� ã�� �� �����ϴ�.");
                return;
            }
            enemy.AnimationEventCalled += EventDecision;
        }
        else
        {
            Debug.LogError("�߸��� ĳ�����Դϴ�.");
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
