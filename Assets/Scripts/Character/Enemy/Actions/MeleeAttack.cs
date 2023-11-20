using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Characters/Enemy/AttackAction/MeleeAttack")]
public class MeleeAttack : AttackAction
{
    [ReadOnly] public Weapon Weapon;
    private AOEIndicator indicator;

    public override void OnAwake()
    {
        base.OnAwake();
        Weapon = StateMachine.Enemy.GetComponentInChildren<Weapon>();
        if (Weapon == null)
        {
            Debug.LogError("Weapon이 필요합니다.");
            return;
        }
        Weapon.WeaponColliderEnter += OnWeaponTriggerEnter;
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
            if (Config.AOEType == AOETypes.None)
                return;
            indicator = AOEIndicatorPool.Instance.GetIndicatorPool(Config.AOEType).Get();
            Transform transform = StateMachine.Enemy.Agent.transform;
            indicator.IndicateAOE(transform.position, transform.forward, Condition.LessThanThisDistance);
        }
        else if (animEvent.stringParameter == "AOEIndicatorOff")
        {
            if (indicator == null)
                return;
            AOEIndicatorPool.Instance.GetIndicatorPool(Config.AOEType).Release(indicator);
        }
    }
    private void OnWeaponTriggerEnter(Collider other)
    {
        ApplyAttack(other.gameObject);
    }
}
