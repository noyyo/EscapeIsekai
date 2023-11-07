using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackEffectTypes
{
    None = 0,
    KnockBack,
    Airborne,
    Stun,
}

[Serializable]
public class ActionConfig
{
    [Tooltip("액션의 효과가 지속되는 시간")]
    public float EffectDurationSeconds;
    public int DamageAmount;
    public AttackEffectTypes AttackEffectType;
    public float AttackEffectValue;
    [SerializeField] private string animTrigger1;
    [SerializeField] private string animTrigger2;
    [HideInInspector] public int AnimTriggerHash1;
    [HideInInspector] public int AnimTriggerHash2;
    [Tooltip("우선순위를 무시하고 이 액션을 바로 이어서 실행합니다.")]
    public AttackAction ChainedAction = null;
    
    public void InitializeAnimHash()
    {
        AnimTriggerHash1 = Animator.StringToHash(animTrigger1);
        AnimTriggerHash2 = Animator.StringToHash(animTrigger2);
    }
}
