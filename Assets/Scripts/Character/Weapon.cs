using System;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class Weapon : MonoBehaviour
{
    [field: SerializeField] public int ID { get; private set; }
    [Header("-- Weapon의 콜라이더는 씬을 시작할 때 Active상태여야 합니다 --")]
    private Collider collider;
    [Tooltip("이 무기를 사용하는 캐릭터의 게임오브젝트를 넣어주어야 합니다.")]
    public GameObject character;
    public event Action<Collider> WeaponColliderEnter;
    [HideInInspector] public Vector3 ColliderSize;
    private static readonly string MeleeAttackStart = "MeleeAttackStart";
    private static readonly string MeleeAttackEnd = "MeleeAttackEnd";

    private void Awake()
    {
        collider = GetComponent<Collider>();
        ColliderSize = collider.bounds.extents * 2;
        collider.enabled = false;
    }

    private void Start()
    {
        if (character == null)
            Debug.LogError("무기를 사용할 캐릭터가 없습니다.");
        if (character.tag == TagsAndLayers.PlayerTag)
        {
            Player player = GetComponentInParent<Player>();
            if (player == null)
            {
                Debug.LogError("Player객체를 찾을 수 없습니다.");
                return;
            }
            player.AnimationEventReceiver.AnimEventCalled += EventDecision;
        }
        else if (character.tag == TagsAndLayers.EnemyTag)
        {
            Enemy enemy = GetComponentInParent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError("Enemy객체를 찾을 수 없습니다.");
                return;
            }
            enemy.AnimEventReceiver.AnimEventCalled += EventDecision;
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
            if (animEvent.intParameter == ID)
                ActivateWeaponCollider();
        }
        else if (animEvent.stringParameter == MeleeAttackEnd)
        {
            if (animEvent.intParameter == ID)
                DeactivateWeaponCollider();
        }
    }
    private void ActivateWeaponCollider()
    {
        collider.enabled = true;
    }
    private void DeactivateWeaponCollider()
    {
        collider.enabled = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        WeaponColliderEnter?.Invoke(other);
    }
}
