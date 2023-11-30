using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Characters/Enemy/AttackAction/MeleeAttack")]
public class MeleeAttack : AttackAction
{
    private Dictionary<int, Weapon> weapons;
    private Weapon weapon;
    [SerializeField] private AOETypes aoeType;
    private AOEIndicator indicator;

    public MeleeAttack()
    {
        ActionType = ActionTypes.MeleeAttack;
    }
    public override void OnAwake()
    {
        base.OnAwake();
        InitializeWeapons();
    }
    public override void OnStart()
    {
        base.OnStart();
        SubscribeWeaponEvent();
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled += AnimEventDecision;
        StartAnimation(Config.AnimTriggerHash1);
    }
    public override void OnEnd()
    {
        base.OnEnd();
        UnsubscribeWeaponEvent();
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled -= AnimEventDecision;
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
    private void AnimEventDecision(AnimationEvent animEvent)
    {
        if (!isRunning)
            return;

        if (animEvent.stringParameter == "AOEIndicatorOn")
        {
            // ID�� ���� weapon ����
            weapon = weapons[animEvent.intParameter];
            if (weapon == null)
            {
                Debug.LogError("�ش��ϴ� ID�� ���� ������ �����ϴ�. �̺�Ʈ�� �Ѱ��ִ� int�Ķ���͸� Ȯ���ϼ���.");
                return;
            }
            if (aoeType == AOETypes.None)
                return;
            indicator = AOEIndicatorPool.Instance.GetIndicatorPool(aoeType).Get();
            Transform transform = StateMachine.Enemy.transform;
            float maxSlopeHeight = Mathf.Sin(NavMesh.GetSettingsByID(StateMachine.Enemy.Agent.agentTypeID).agentSlope) * Condition.LessThanThisDistance;

            if (aoeType == AOETypes.Box)
            {
                Vector3 indicatorPosition = transform.TransformPoint(new Vector3(0, 0, Condition.LessThanThisDistance));
                indicatorPosition.y += maxSlopeHeight;
                indicator.IndicateBoxAOE(indicatorPosition, transform.forward, weapon.ColliderSize.x, Condition.LessThanThisDistance - Condition.MoreThanThisDistance, maxSlopeHeight * 2);
            }
            else
            {
                Vector3 indicatorPosition = transform.position;
                indicatorPosition.y += maxSlopeHeight;
                indicator.IndicateCircleAOE(indicatorPosition, transform.forward, Condition.LessThanThisDistance, maxSlopeHeight * 2);
            }
        }
        else if (animEvent.stringParameter == "AOEIndicatorOff")
        {
            if (indicator == null)
            {
                Debug.LogError("MeleeAttack�� Indicator�� �����ϴ�.");
            }
            AOEIndicatorPool.Instance.GetIndicatorPool(aoeType).Release(indicator);
        }
    }
    private void InitializeWeapons()
    {
        weapons = new Dictionary<int, Weapon>();
        foreach(Weapon weapon in StateMachine.Enemy.GetComponentsInChildren<Weapon>())
        {
            weapons.Add(weapon.ID, weapon);
        }
        if (weapons.Count == 0)
        {
            Debug.LogError("Weapon�� �ʿ��մϴ�.");
            return;
        }
    }
    private void SubscribeWeaponEvent()
    {
        foreach (Weapon weapon in weapons.Values)
        {
            weapon.WeaponColliderEnter += OnWeaponTriggerEnter;
        }
    }
    private void UnsubscribeWeaponEvent()
    {
        foreach (Weapon weapon in weapons.Values)
        {
            weapon.WeaponColliderEnter -= OnWeaponTriggerEnter;
        }
    }
    private void OnWeaponTriggerEnter(Collider other)
    {
        ApplyAttack(other.gameObject);
    }
    protected override void OnDrawGizmo(Transform enemyTransform)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemyTransform.position, Condition.LessThanThisDistance);
    }
}
