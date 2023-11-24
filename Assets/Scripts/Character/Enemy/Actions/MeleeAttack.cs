using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Characters/Enemy/AttackAction/MeleeAttack")]
public class MeleeAttack : AttackAction
{
    [SerializeField][ReadOnly] private Weapon weapon;
    [SerializeField] private AOETypes aoeType;
    private AOEIndicator indicator;

    public MeleeAttack()
    {
        ActionType = ActionTypes.MeleeAttack;
    }
    public override void OnAwake()
    {
        base.OnAwake();
        weapon = StateMachine.Enemy.GetComponentInChildren<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("Weapon이 필요합니다.");
            return;
        }
        weapon.WeaponColliderEnter += OnWeaponTriggerEnter;
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled += AnimEventDecision;
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
    private void AnimEventDecision(AnimationEvent animEvent)
    {
        if (!isRunning)
            return;

        if (animEvent.stringParameter == "AOEIndicatorOn")
        {
            if (aoeType == AOETypes.None)
                return;
            indicator = AOEIndicatorPool.Instance.GetIndicatorPool(aoeType).Get();
            Transform transform = StateMachine.Enemy.transform;
            float maxSlopeHeight = Mathf.Sin(NavMesh.GetSettingsByID(StateMachine.Enemy.Agent.agentTypeID).agentSlope) * Condition.LessThanThisDistance;
            Vector3 indicatorPosition = transform.position;
            indicatorPosition.y += maxSlopeHeight;
            indicator.IndicateCircleAOE(indicatorPosition, transform.forward, Condition.LessThanThisDistance, maxSlopeHeight * 2);
        }
        else if (animEvent.stringParameter == "AOEIndicatorOff")
        {
            if (indicator == null)
                return;
            AOEIndicatorPool.Instance.GetIndicatorPool(aoeType).Release(indicator);
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
