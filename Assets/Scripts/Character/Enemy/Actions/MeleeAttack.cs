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
        weapons = StateMachine.Enemy.Weapons;
        if (weapons.Count == 0)
        {
            Debug.LogError("Weapon이 필요합니다.");
            return;
        }
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
            // ID를 통해 weapon 선택
            weapon = weapons[animEvent.intParameter];
            if (weapon == null)
            {
                Debug.LogError("해당하는 ID를 가진 웨폰이 없습니다. 이벤트로 넘겨주는 int파라미터를 확인하세요.");
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
                Debug.LogError("MeleeAttack의 Indicator가 없습니다.");
            }
            AOEIndicatorPool.Instance.GetIndicatorPool(aoeType).Release(indicator);
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
