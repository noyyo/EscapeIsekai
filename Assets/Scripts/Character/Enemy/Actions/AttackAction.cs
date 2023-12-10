using System.Collections.Generic;
using UnityEngine;

public enum ActionTypes
{
    None = -1,
    MeleeAttack,
    Breath,
    LaunchProjectile,
    CharingAttack,
    ProjectileRain,
    HideAndAttack,
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
    public ActionTypes ActionType;
    [SerializeField]
    public ActionConfig Config;
    [SerializeField]
    public ActionCondition Condition;
    protected float timeStarted { get; set; }
    protected float timeRunning { get { return (Time.time - timeStarted); } }
    protected bool isRunning;
    /// <summary>
    /// 액션이 완료되면 true로 세팅해야합니다. true값이 되면 다음 프레임에서 액션의 실행은 종료됩니다.
    /// </summary>
    protected bool isCompleted;
    private bool isInterrupted;
    /// <summary>
    /// 이펙트가 시작됐는지 여부입니다.
    /// </summary>
    protected bool isEffectStarted;
    /// <summary>
    /// 이펙트가 끝났는지 여부입니다.
    /// </summary>
    protected bool isEffectEnded;
    /// <summary>
    /// 이펙트를 시작한 시간입니다. OnEffectStart를 실행하면 자동으로 저장됩니다.
    /// </summary>
    [HideInInspector] public float EffectStartTime;
    /// <summary>
    /// 현재 액션은 끝났지만 효과가 남아있는 경우 true입니다.
    /// </summary>
    [HideInInspector] public bool HasRemainingEffect;
    /// <summary>
    /// 애니메이션 상태를 관리하는 딕셔너리입니다. Hash값을 통해 현재 상태를 불러올 수 있습니다.
    /// </summary>
    protected Dictionary<int, AnimState> animState = new Dictionary<int, AnimState>(2);
    /// <summary>
    /// 현재 실행되고 있는 애니메이션의 Hash값입니다. 실행중인 애니메이션이 없다면 0입니다.
    /// </summary>
    protected int currentAnimHash;
    /// <summary>
    /// 현재 실행되고 있는 애니메이션의 진행시간입니다. Transition이 있는 경우 원하는 값과 달라질 수 있습니다.
    /// </summary>
    protected float currentAnimNormalizedTime;
    private AnimatorStateInfo lastAnimStateInfo;
    private bool isAnimStarted;
    [HideInInspector]
    public float lastUsedTime;
    private float restedTime;


    private HashSet<GameObject> alreadyAttackApplied = new HashSet<GameObject>();

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
        isInterrupted = false;
        isEffectStarted = false;
        isEffectEnded = false;
        alreadyAttackApplied.Clear();
        isRunning = true;
        restedTime = 0f;
    }
    /// <summary>
    /// 매 프레임마다 호출됩니다.
    /// </summary>
    public virtual void OnUpdate()
    {
        if (isEffectStarted && !isEffectEnded && Time.time - EffectStartTime >= Config.EffectDurationSeconds)
        {
            OnEffectFinish();
        }
        if (!isCompleted && !isInterrupted)
            CheckAnimationState();

        if (isCompleted && !HasRemainingEffect && !isInterrupted)
        {
            if (restedTime < Config.RestTimeAfterAction)
            {
                restedTime += Time.deltaTime;
                return;
            }
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
    /// isCompleted값이 true가 되면 다음 프레임에서 자동으로 호출됩니다.
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
            isRunning = false;
        }
        animState[Config.AnimTriggerHash1] = AnimState.NotStarted;
        animState[Config.AnimTriggerHash2] = AnimState.NotStarted;
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
        if (HasRemainingEffect)
        {
            StateMachine.RemoveActionInActive(this);
            HasRemainingEffect = false;
            lastUsedTime = Time.time;
            isRunning = false;
        }
    }
    /// <summary>
    /// 공격을 적용할 때 한 번만 호출해주어야 합니다.
    /// </summary>
    /// <param name="target">데미지 및 공격 효과를 적용할 대상입니다.</param>
    /// <param name="isPossibleMultihit">이미 공격이 적용된 대상에게도 다시 공격을 적용할 수 있는지 여부입니다.</param>
    /// <param name="targetObj">공격을 이미 받았는지 판단하기 위한 대상의 GameObject입니다.</param>
    protected void ApplyAttack(GameObject targetObj, bool isPossibleMultihit = false, bool isPossibleMultiEffect = false)
    {
        if (!isPossibleMultihit && !isPossibleMultiEffect && alreadyAttackApplied.Contains(targetObj))
            return;
        if (targetObj == StateMachine.Enemy)
            return;
        IDamageable target = GetDamageableComponent(targetObj);
        if (target == null)
            return;
        if (isPossibleMultiEffect && alreadyAttackApplied.Contains(targetObj))
        {
            target.TakeEffect(Config.AttackEffectType, Config.AttackEffectValue, StateMachine.Enemy.gameObject);
            return;
        }
        target.TakeDamage(Config.DamageAmount, StateMachine.Enemy.gameObject);
        target.TakeEffect(Config.AttackEffectType, Config.AttackEffectValue, StateMachine.Enemy.gameObject);
        alreadyAttackApplied.Add(targetObj);
    }
    private IDamageable GetDamageableComponent(GameObject targetObj)
    {
        IDamageable target = null;
        if (targetObj.tag == TagsAndLayers.PlayerTag)
        {
            Player player;
            targetObj.TryGetComponent(out player);
            if (player == null)
            {
                Debug.LogError("Player 스크립트를 찾을 수 없습니다.");
                return null;
            }
            target = player.StateMachine;
        }
        else if (targetObj.transform.tag == TagsAndLayers.EnemyTag)
        {
            Enemy enemy;
            targetObj.TryGetComponent(out enemy);
            if (enemy == null)
            {
                Debug.LogError("Enemy 스크립트를 찾을 수 없습니다.");
                return null;
            }
            target = enemy.StateMachine;
        }
        else if (targetObj.tag == TagsAndLayers.EnvironmentTag)
        {
            BaseEnvironmentObject environmentObj;
            targetObj.TryGetComponent(out environmentObj);
            if (environmentObj == null)
            {
                Debug.LogError("대상에게 BaseEnvironmentObject 컴포넌트가 없습니다.");
                return null;
            }
            target = environmentObj;
        }
        return target;
    }
    private void InitializeAnimState()
    {
        if (Config.AnimTriggerHash1 != 0)
            animState.Add(Config.AnimTriggerHash1, AnimState.NotStarted);
        if (Config.AnimTriggerHash2 != 0)
            animState.Add(Config.AnimTriggerHash2, AnimState.NotStarted);
    }
    /// <summary>
    /// 애니메이션을 시작하고 상태를 관리합니다.
    /// 애니메이션이 Bool값으로 실행되는 경우 StopAnimation을 로직에 따라 호출해주어야 합니다.
    /// 그렇지 않을 경우 애니메이션이 무한히 실행될 수 있습니다.
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
    /// 애니메이션이 Bool값으로 실행되는 경우 Bool값을 false로 세팅합니다.
    /// 이외의 경우에는 아무것도 하지 않습니다.
    /// </summary>
    /// <param name="animTriggerHash">Bool타입의 파라미터여야 합니다.</param>
    protected void StopAnimation(int animTriggerHash)
    {
        foreach (AnimatorControllerParameter param in StateMachine.Animator.parameters)
        {
            if (param.nameHash == animTriggerHash)
            {
                if (param.type != AnimatorControllerParameterType.Bool)
                    return;
            }
        }
        StateMachine.Animator.SetBool(animTriggerHash, false);
        currentAnimHash = 0;
        currentAnimNormalizedTime = 0f;
        isAnimStarted = false;
        animState[animTriggerHash] = AnimState.NotStarted;
    }
    private void CheckAnimationState()
    {
        if (currentAnimHash == 0)
            return;

        if (animState[currentAnimHash] == AnimState.Playing)
        {
            AnimatorStateInfo currentInfo = StateMachine.Animator.GetCurrentAnimatorStateInfo(0);

            if (!isAnimStarted && lastAnimStateInfo.fullPathHash != currentInfo.fullPathHash)
            {
                if (currentInfo.IsTag("BattleStance"))
                    return;
                lastAnimStateInfo = currentInfo;
                currentAnimNormalizedTime = currentInfo.normalizedTime;
                isAnimStarted = true;
            }
            else if (isAnimStarted && lastAnimStateInfo.fullPathHash == currentInfo.fullPathHash && currentInfo.normalizedTime < 1f)
            {
                currentAnimNormalizedTime = currentInfo.normalizedTime;
            }
            else if (isAnimStarted && (lastAnimStateInfo.fullPathHash != currentInfo.fullPathHash || currentInfo.normalizedTime > 1f))
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
    public void Interrupt()
    {
        isInterrupted = true;
        ReleaseIndicator();
    }
    protected abstract void ReleaseIndicator();
    protected virtual void OnDrawGizmo(Transform enemyTransform) { }
}
