using System;
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
    /// <summary>
    /// 해당 필드는 인스펙터에서 편집이 불가능합니다. 파생된 클래스에서 지정해주어야 합니다.
    /// </summary>
    [ReadOnly] public ActionTypes ActionType;
    [SerializeField]
    public ActionConfig Config;
    [SerializeField]
    public ActionCondition Condition;
    protected float timeStarted { get; set; }
    protected float timeRunning { get { return (Time.time - timeStarted); } }
    /// <summary>
    /// 액션이 완료되면 true로 세팅해야합니다. true값이 되면 다음 프레임에서 액션의 실행은 종료됩니다.
    /// </summary>
    protected bool isCompleted;
    protected bool isEffectStarted;
    protected bool isEffectEnded;
    public float EffectStartTime;
    protected bool hasRemainingEffect = false;
    /// <summary>
    /// 애니메이션 상태를 관리하는 딕셔너리입니다. Hash값을 통해 현재 상태를 불러올 수 있습니다.
    /// !! 주의사항 : 트리거를 통해서 실행되는 애니메이션이 없다면 올바른 값이 아닐 수 있습니다.
    /// </summary>
    protected Dictionary<int, AnimState> animState = new Dictionary<int, AnimState>(2);
    protected int currentAnimHash;
    protected float currentAnimNormalizedTime;
    private bool isCurrentAnimParameterBool = false;
    [HideInInspector]
    public float lastUsedTime;

    public void SetStateMachine(EnemyStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
    /// <summary>
    /// 초기화를 위해 가장 먼저 실행되며 단 한 번만 호출됩니다.
    /// </summary>
    public virtual void OnAwake()
    {
        Condition.Initialize(this);
        Config.InitializeAnimHash();
        InitializeAnimState();
    }
    /// <summary>
    /// 액션을 시작할 때마다 한 번 호출됩니다.
    /// </summary>
    public virtual void OnStart()
    {
        timeStarted = Time.time;
        isCompleted = false;
        isEffectStarted = false;
        isEffectEnded = false;
    }
    /// <summary>
    /// 매 프레임마다 호출됩니다.
    /// </summary>
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
        
        if (isEffectStarted && !isEffectEnded && Time.time - EffectStartTime >= Config.EffectDurationSeconds)
        {
            OnEffectFinish();
        }
        CheckAnimationState();

    }
    /// <summary>
    /// isCompleted값이 true가 되면 다음 프레임에서 자동으로 호출됩니다.
    /// </summary>
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
    /// <summary>
    /// Effect를 시작할 때 호출해주어야 합니다. 자동으로 호출되지 않습니다.
    /// </summary>
    protected virtual void OnEffectStart()
    {
        EffectStartTime = Time.time;
        isEffectStarted = true;
    }
    /// <summary>
    /// Effect를 시작하고 Duration만큼의 시간이 지났다면 자동으로 호출됩니다.
    /// 만약 isCompleted가 true가 되었음에도 Effect가 끝나지 않았다면 StateMachine의 활성화된 액션 목록에 추가됩니다.
    /// </summary>
    protected virtual void OnEffectFinish()
    {
        isEffectEnded = true;
        if (hasRemainingEffect)
        {
            StateMachine.RemoveActionInActive(this);
            lastUsedTime = Time.time;
        }
    }
    /// <summary>
    /// 공격을 적용할 때 한 번만 호출해주어야 합니다.
    /// </summary>
    /// <param name="target">데미지 및 공격 효과를 적용할 대상입니다.</param>
    protected void ApplyAttack(IDamageable target)
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
                    // TODO : 스턴 효과 적용
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
    private void InitializeAnimState()
    {
        animState.Add(Config.AnimTriggerHash1, AnimState.NotStarted);
        animState.Add(Config.AnimTriggerHash2, AnimState.NotStarted);
    }
    /// <summary>
    /// 애니메이션을 시작하고 상태를 관리합니다.
    /// 애니메이션이 Bool값으로 실행되는 경우 StopAnimation을 로직에 따라 호출해주어야 합니다.
    /// </summary>
    /// <param name="animTriggerHash"></param>
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
        AnimatorControllerParameterType paramType = StateMachine.Animator.GetParameter(currentAnimHash).type;
        if (paramType == AnimatorControllerParameterType.Trigger)
        {
            StateMachine.Animator.SetTrigger(animTriggerHash);
        }
        else if (paramType == AnimatorControllerParameterType.Bool)
        {
            StateMachine.Animator.SetBool(animTriggerHash, true);
            isCurrentAnimParameterBool = true;
        }
        animState[animTriggerHash] = AnimState.Playing;
        currentAnimHash = animTriggerHash;
    }
    /// <summary>
    /// 애니메이션이 Bool값으로 실행되는 경우 Bool값을 false로 세팅합니다.
    /// 이외의 경우에는 아무것도 하지 않습니다.
    /// </summary>
    /// <param name="animTriggerHash">Bool타입의 파라미터여야 합니다.</param>
    protected void StopAnimation(int animTriggerHash)
    {
        AnimatorControllerParameterType paramType = StateMachine.Animator.GetParameter(animTriggerHash).type;
        if (paramType == AnimatorControllerParameterType.Bool)
        {
            StateMachine.Animator.SetBool(animTriggerHash, false);
            isCurrentAnimParameterBool = false;
        }
    }
    private void CheckAnimationState()
    {
        if (currentAnimHash == 0)
            return;

        if (animState[currentAnimHash] == AnimState.Playing)
        {
            float normalizedTime = StateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (normalizedTime <= currentAnimNormalizedTime)
            {
                if (isCurrentAnimParameterBool)
                {
                    currentAnimNormalizedTime = normalizedTime;
                    return;
                }
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
