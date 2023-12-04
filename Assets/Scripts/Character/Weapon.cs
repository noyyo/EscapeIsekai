using System;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class Weapon : MonoBehaviour
{
    [field: SerializeField] public int ID { get; private set; }
    [Header("-- Weapon�� �ݶ��̴��� ���� ������ �� Active���¿��� �մϴ� --")]
    private Collider collider;
    [Tooltip("�� ���⸦ ����ϴ� ĳ������ ���ӿ�����Ʈ�� �־��־�� �մϴ�.")]
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
            Debug.LogError("���⸦ ����� ĳ���Ͱ� �����ϴ�.");
        if (character.tag == TagsAndLayers.PlayerTag)
        {
            Player player = GetComponentInParent<Player>();
            if (player == null)
            {
                Debug.LogError("Player��ü�� ã�� �� �����ϴ�.");
                return;
            }
            player.AnimationEventReceiver.AnimEventCalled += EventDecision;
        }
        else if (character.tag == TagsAndLayers.EnemyTag)
        {
            Enemy enemy = GetComponentInParent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError("Enemy��ü�� ã�� �� �����ϴ�.");
                return;
            }
            enemy.AnimEventReceiver.AnimEventCalled += EventDecision;
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
