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
    protected bool isEffectStarted;
    protected bool isEffectEnded;
    public float EffectStartTime;
    protected bool hasRemainingEffect = false;
    /// <summary>
    /// �ִϸ��̼� ���¸� �����ϴ� ��ųʸ��Դϴ�. Hash���� ���� ���� ���¸� �ҷ��� �� �ֽ��ϴ�.
    /// !! ���ǻ��� : Ʈ���Ÿ� ���ؼ� ����Ǵ� �ִϸ��̼��� ���ٸ� �ùٸ� ���� �ƴ� �� �ֽ��ϴ�.
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
    /// isCompleted���� true�� �Ǹ� ���� �����ӿ��� �ڵ����� ȣ��˴ϴ�.
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
        if (hasRemainingEffect)
        {
            StateMachine.RemoveActionInActive(this);
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
