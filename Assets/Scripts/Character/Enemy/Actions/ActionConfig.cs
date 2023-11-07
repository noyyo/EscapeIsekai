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
    [Tooltip("�׼��� ȿ���� ���ӵǴ� �ð�")]
    public float EffectDurationSeconds;
    public int DamageAmount;
    public AttackEffectTypes AttackEffectType;
    public float AttackEffectValue;
    [SerializeField] private string animTrigger1;
    [SerializeField] private string animTrigger2;
    [HideInInspector] public int AnimTriggerHash1;
    [HideInInspector] public int AnimTriggerHash2;
    [Tooltip("�켱������ �����ϰ� �� �׼��� �ٷ� �̾ �����մϴ�.")]
    public AttackAction ChainedAction = null;
    
    public void InitializeAnimHash()
    {
        AnimTriggerHash1 = Animator.StringToHash(animTrigger1);
        AnimTriggerHash2 = Animator.StringToHash(animTrigger2);
    }
}