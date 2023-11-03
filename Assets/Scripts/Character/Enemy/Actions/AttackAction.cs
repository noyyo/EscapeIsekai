using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public enum ActionTypes
{
    None = -1,
    Melee,
    Target,
    RangedTargeted,
    LaunchProjectile,
    ChargedLaunchProjectile,
    AoE,
}
public enum AnimState
{
    NotStarted,
    Playing,
    Completed
}


[CreateAssetMenu(fileName = "AttackActionSO", menuName = "Characters/Enemy/AttackAction")]
public abstract class AttackAction : ScriptableObject
{
    [HideInInspector]
    public EnemyStateMachine StateMachine;
    [ReadOnly] // 해당 필드는 인스펙터에서 편집이 불가능합니다. 파생된 클래스에서 지정합니다.
    public ActionTypes ActionType;
    [SerializeField]
    public ActionConfig Config;
    [SerializeField]
    public ActionCondition Condition;
    protected float timeStarted { get; set; }
    protected float timeRunning { get { return (Time.time - timeStarted); } }
    // 액션이 완료되면 true로 세팅해줘야 합니다.
    protected bool isCompleted;
    protected bool isEffectStarted;
    protected bool isEffectEnded;
    protected float effectStartTime;
    protected bool hasRemainingEffect = false;
    // 애니메이션이 실행됐는지 여부를 확인하는 딕셔너리입니다.
    protected Dictionary<int, AnimState> animState = new Dictionary<int, AnimState>(2);
    protected int currentAnimHash;
    protected float currentAnimNormalizedTime;
    [HideInInspector]
    public float lastUsedTime;
    
    public virtual void OnAwake()
    {
        Config.InitializeAnimHash();
        InitializeAnimState();
    }
    public virtual void OnStart()
    {
        timeStarted = Time.time;
        isCompleted = false;
        isEffectStarted = false;
        isEffectEnded = false;
    }
    public virtual void OnUpdate()
    {
        if (isCompleted)
        {
            if (Config.ChainedAction != null)
            {
                StateMachine.CurrentAction = Config.ChainedAction;
                StateMachine.ChangeState(StateMachine.AttackState);
            }
            else
            {
                StateMachine.ChangeState(StateMachine.ChaseState);
            }
            return;
        }
        
        if (isEffectStarted && !isEffectEnded && Time.time - effectStartTime >= Config.EffectDurationSeconds)
        {
            OnEffectFinish();
        }
        CheckAnimationState();

    }
    public virtual void OnEnd()
    {
        if (isEffectStarted && !isEffectEnded)
        {
            hasRemainingEffect = true;
            StateMachine.AddActionInActive(this);
        }
        else
        {
            lastUsedTime = Time.time;
        }
    }
    protected virtual void OnEffectStart()
    {
        effectStartTime = Time.time;
        isEffectStarted = true;
    }
    protected virtual void OnEffectFinish()
    {
        isEffectEnded = true;
        if (hasRemainingEffect)
        {
            StateMachine.RemoveActionInActive(this);
            lastUsedTime = Time.time;
        }
    }
    protected virtual void ApplyAttack(IDamageable target)
    {
        target.TakeDamage(Config.DamageAmount);
        if (target.CanTakeAttackEffect)
        {
            MonoBehaviour targetObject;
            if (!(target is MonoBehaviour))
                return;
            targetObject = target as MonoBehaviour;
            switch (Config.AttackEffectType)
            {
                case AttackEffectTypes.None:
                    break;
                case AttackEffectTypes.KnockBack:
                    ApplyKnockBackOrAirborne(targetObject.gameObject, AttackEffectTypes.KnockBack);
                    break;
                case AttackEffectTypes.Airborne:
                    ApplyKnockBackOrAirborne(targetObject.gameObject, AttackEffectTypes.Airborne);
                    break;
                case AttackEffectTypes.Stun:
                    break;
            }
        }
    }
    private void ApplyKnockBackOrAirborne(GameObject target, AttackEffectTypes effectType)
    {
        if (target == null)
            return;
        Transform targetTransform = target.transform;
        Rigidbody rigidbody;
        if (target.TryGetComponent(out rigidbody))
        {
            Vector3 direction = Vector3.zero;
            switch (effectType)
            {
                case AttackEffectTypes.KnockBack:
                    direction = targetTransform.forward;
                    rigidbody.AddForce(direction.normalized * Config.AttackEffectValue);
                    break;
                case AttackEffectTypes.Airborne:
                    direction = Vector3.up;
                    rigidbody.AddForce(direction * Config.AttackEffectValue);
                    break;
                default:
                    return;
            }

        }
    }
    protected void InitializeAnimState()
    {
        animState.Add(Config.AnimTriggerHash1, AnimState.NotStarted);
        animState.Add(Config.AnimTriggerHash2, AnimState.NotStarted);
    }
    protected void StartAnimation(int animTriggerHash)
    {
        if (!animState.ContainsKey(animTriggerHash))
        {
            Debug.LogError("해당 애니메이션 해쉬가 없습니다.");
            return;
        }
            
        foreach (AnimState state in animState.Values)
        {
            if (state == AnimState.Playing)
            {
                Debug.LogError("이미 다른 애니메이션이 실행중입니다.");
                return;
            }
        }
        StateMachine.Animator.SetTrigger(animTriggerHash);
        animState[animTriggerHash] = AnimState.Playing;
        currentAnimHash = animTriggerHash;
    }
    protected void CheckAnimationState()
    {
        if (animState[currentAnimHash] == AnimState.Playing)
        {
            float normalizedTime = StateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (normalizedTime < currentAnimNormalizedTime)
            {
                animState[currentAnimHash] = AnimState.Completed;
                currentAnimNormalizedTime = 0f;
            }
            else
            {
                currentAnimNormalizedTime = normalizedTime;
            }
        }
    }
    
}
