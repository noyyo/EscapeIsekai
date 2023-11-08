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
    /// �ش� �ʵ�� �ν����Ϳ��� ������ �Ұ����մϴ�. �Ļ��� Ŭ�������� �������־�� �մϴ�.
    /// </summary>
    [ReadOnly] public ActionTypes ActionType;
    [SerializeField]
    public ActionConfig Config;
    [SerializeField]
    public ActionCondition Condition;
    protected float timeStarted { get; set; }
    protected float timeRunning { get { return (Time.time - timeStarted); } }
    /// <summary>
    /// �׼��� �Ϸ�Ǹ� true�� �����ؾ��մϴ�. true���� �Ǹ� ���� �����ӿ��� �׼��� ������ ����˴ϴ�.
    /// </summary>
    protected bool isCompleted;
    /// <summary>
    /// ����Ʈ�� ���۵ƴ��� �����Դϴ�.
    /// </summary>
    protected bool isEffectStarted;
    /// <summary>
    /// ����Ʈ�� �������� �����Դϴ�.
    /// </summary>
    protected bool isEffectEnded;
    /// <summary>
    /// ����Ʈ�� ������ �ð��Դϴ�. OnEffectStart�� �����ϸ� �ڵ����� ����˴ϴ�.
    /// </summary>
    [HideInInspector] public float EffectStartTime;
    /// <summary>
    /// ���� �׼��� �������� ȿ���� �����ִ� ��� true�Դϴ�.
    /// </summary>
    [HideInInspector] public bool HasRemainingEffect;
    /// <summary>
    /// �ִϸ��̼� ���¸� �����ϴ� ��ųʸ��Դϴ�. Hash���� ���� ���� ���¸� �ҷ��� �� �ֽ��ϴ�.
    /// </summary>
    protected Dictionary<int, AnimState> animState = new Dictionary<int, AnimState>(2);
    /// <summary>
    /// ���� ����ǰ� �ִ� �ִϸ��̼��� Hash���Դϴ�. �������� �ִϸ��̼��� ���ٸ� 0�Դϴ�.
    /// </summary>
    protected int currentAnimHash;
    /// <summary>
    /// ���� ����ǰ� �ִ� �ִϸ��̼��� ����ð��Դϴ�. Transition�� �ִ� ��� ���ϴ� ���� �޶��� �� �ֽ��ϴ�.
    /// </summary>
    protected float currentAnimNormalizedTime;
    private AnimatorStateInfo lastAnimStateInfo;
    private bool isAnimStarted;
    [HideInInspector]
    public float lastUsedTime;

    public void SetStateMachine(EnemyStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
    /// <summary>
    /// �ʱ�ȭ�� ���� ���� ���� ����Ǹ� �� �� ���� ȣ��˴ϴ�.
    /// </summary>
    public virtual void OnAwake()
    {
        Condition.Initialize(this);
        Config.InitializeAnimHash();
        InitializeAnimState();
    }
    /// <summary>
    /// �׼��� ������ ������ �� �� ȣ��˴ϴ�.
    /// </summary>
    public virtual void OnStart()
    {
        timeStarted = Time.time;
        isCompleted = false;
        isEffectStarted = false;
        isEffectEnded = false;
    }
    /// <summary>
    /// �� �����Ӹ��� ȣ��˴ϴ�.
    /// </summary>
    public virtual void OnUpdate()
    {
        if (isEffectStarted && !isEffectEnded && Time.time - EffectStartTime >= Config.EffectDurationSeconds)
        {
            OnEffectFinish();
        }
        CheckAnimationState();

        if (isCompleted && !HasRemainingEffect)
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
        }
    }
    /// <summary>
    /// isCompleted���� true�� �Ǹ� ���� �����ӿ��� �ڵ����� ȣ��˴ϴ�.
    /// </summary>
    public virtual void OnEnd()
    {
        if (isEffectStarted && !isEffectEnded)
        {
            HasRemainingEffect = true;
            StateMachine.AddActionInActive(this);
        }
        else
        {
            lastUsedTime = Time.time;
        }
        animState[Config.AnimTriggerHash1] = AnimState.NotStarted;
        animState[Config.AnimTriggerHash2] = AnimState.NotStarted;
    }
    /// <summary>
    /// Effect�� ������ �� ȣ�����־�� �մϴ�. �ڵ����� ȣ����� �ʽ��ϴ�.
    /// </summary>
    protected virtual void OnEffectStart()
    {
        EffectStartTime = Time.time;
        isEffectStarted = true;
    }
    /// <summary>
    /// Effect�� �����ϰ� Duration��ŭ�� �ð��� �����ٸ� �ڵ����� ȣ��˴ϴ�.
    /// ���� isCompleted�� true�� �Ǿ������� Effect�� ������ �ʾҴٸ� StateMachine�� Ȱ��ȭ�� �׼� ��Ͽ� �߰��˴ϴ�.
    /// </summary>
    protected virtual void OnEffectFinish()
    {
        isEffectEnded = true;
        if (HasRemainingEffect)
        {
            StateMachine.RemoveActionInActive(this);
            HasRemainingEffect = false;
            lastUsedTime = Time.time;
        }
    }
    /// <summary>
    /// ������ ������ �� �� ���� ȣ�����־�� �մϴ�.
    /// </summary>
    /// <param name="target">������ �� ���� ȿ���� ������ ����Դϴ�.</param>
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
                    // TODO : ���� ȿ�� ����
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
    /// �ִϸ��̼��� �����ϰ� ���¸� �����մϴ�.
    /// �ִϸ��̼��� Bool������ ����Ǵ� ��� StopAnimation�� ������ ���� ȣ�����־�� �մϴ�.
    /// �׷��� ���� ��� �ִϸ��̼��� ������ ����� �� �ֽ��ϴ�.
    /// </summary>
    /// <param name="animTriggerHash"></param>
    protected void StartAnimation(int animTriggerHash)
    {
        if (!animState.ContainsKey(animTriggerHash))
        {
            Debug.LogError("�ش� �ִϸ��̼� �ؽ��� �����ϴ�.");
            return;
        }
            
        foreach (AnimState state in animState.Values)
        {
            if (state == AnimState.Playing)
            {
                Debug.LogError("�̹� �ٸ� �ִϸ��̼��� �������Դϴ�.");
                return;
            }
        }
        AnimatorControllerParameterType paramType = AnimatorControllerParameterType.Trigger;
        foreach (AnimatorControllerParameter param in StateMachine.Animator.parameters)
        {
            if (param.nameHash == animTriggerHash)
                paramType = param.type;
        }
        if (paramType == AnimatorControllerParameterType.Trigger)
        {
            StateMachine.Animator.SetTrigger(animTriggerHash);
        }
        else if (paramType == AnimatorControllerParameterType.Bool)
        {
            StateMachine.Animator.SetBool(animTriggerHash, true);
        }
        animState[animTriggerHash] = AnimState.Playing;
        currentAnimHash = animTriggerHash;
        lastAnimStateInfo = StateMachine.Animator.GetCurrentAnimatorStateInfo(0);
    }
    /// <summary>
    /// �ִϸ��̼��� Bool������ ����Ǵ� ��� Bool���� false�� �����մϴ�.
    /// �̿��� ��쿡�� �ƹ��͵� ���� �ʽ��ϴ�.
    /// </summary>
    /// <param name="animTriggerHash">BoolŸ���� �Ķ���Ϳ��� �մϴ�.</param>
    protected void StopAnimation(int animTriggerHash)
    {
        AnimatorControllerParameterType paramType = StateMachine.Animator.GetParameter(animTriggerHash).type;
        if (paramType == AnimatorControllerParameterType.Bool)
        {
            StateMachine.Animator.SetBool(animTriggerHash, false);
        }
    }
    private void CheckAnimationState()
    {
        if (currentAnimHash == 0)
            return;

        if (animState[currentAnimHash] == AnimState.Playing)
        {
            AnimatorStateInfo currentInfo = StateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            if (!isAnimStarted && lastAnimStateInfo.fullPathHash != currentInfo.fullPathHash || (lastAnimStateInfo.fullPathHash == currentInfo.fullPathHash && lastAnimStateInfo.normalizedTime >= currentInfo.normalizedTime))
            {
                lastAnimStateInfo = currentInfo;
                currentAnimNormalizedTime = currentInfo.normalizedTime;
                isAnimStarted = true;
            }
            else if (lastAnimStateInfo.fullPathHash == currentInfo.fullPathHash)
            {
                currentAnimNormalizedTime = currentInfo.normalizedTime;
            }
            else if (isAnimStarted && lastAnimStateInfo.fullPathHash != currentInfo.fullPathHash || (lastAnimStateInfo.fullPathHash == currentInfo.fullPathHash && lastAnimStateInfo.normalizedTime >= currentInfo.normalizedTime))
            {
                if (StateMachine.Animator.IsInTransition(0))
                {
                    currentAnimNormalizedTime = lastAnimStateInfo.normalizedTime;
                }
                else
                {
                    animState[currentAnimHash] = AnimState.Completed;
                    currentAnimHash = 0;
                    currentAnimNormalizedTime = 0f;
                    isAnimStarted = false;
                }
            }
        }
    }
    
}
