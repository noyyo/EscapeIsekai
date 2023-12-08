using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

[Serializable]
public class EnemyStateMachine : StateMachine, IDamageable
{
    public event Action<bool> IsPauseChanged;
    public event Action ActivatedActionsChanged;
    public event Action OnDie;
    public event Action<Enemy> OnDieAction;

    // ���� �Ŵ������� �÷��̾� �ҷ���.
    public GameObject Player { get; }
    public IPositionable PositionableTarget;
    public Enemy Enemy { get; }
    public Animator Animator { get; private set; }
    public float MovementSpeedModifier { get; set; } = 1f;
    private EnemyForceReceiver forceReceiver;
    private NavMeshAgent agent;
    public EnemyIdleState IdleState { get; }
    public EnemyWanderState WanderState { get; }
    public EnemyChaseState ChaseState { get; }
    public EnemyAttackState AttackState { get; }
    public EnemyReturnToBaseState ReturnToBaseState { get; }
    public EnemyFleeState FleeState { get; }
    public EnemyDeadState DeadState { get; }
    public EnemyStunState StunState { get; }

    [SerializeField][ReadOnly] private bool isPause;
    [SerializeField][ReadOnly] private bool isActive;
    [SerializeField] private float ActivationCheckDelay = 0.5f;
    [SerializeField] private float ActivationDistance = 100f;
    private float lastCheckTime;
    public float TargetDistance { get; private set; }
    public Vector3 OriginPosition { get; set; }
    [field: SerializeField][field: ReadOnly] public bool IsInvincible { get; set; }
    [field: SerializeField][field: ReadOnly] public bool IsFleeable { get; set; }
    private AffectedAttackEffectInfo affectedEffectInfo = new AffectedAttackEffectInfo();
    public AffectedAttackEffectInfo AffectedEffectInfo { get => affectedEffectInfo; }

    [ReadOnly] public AttackAction CurrentAction;
    private AttackAction[] actionData;
    [SerializeField][ReadOnly] private List<AttackAction> actionsInActive = new List<AttackAction>(5);
    [SerializeField][ReadOnly] private List<AttackAction> actionsToExecute;
    [field: SerializeField] public int HP { get; private set; }
    [ReadOnly] public float BattleTime;
    [ReadOnly] public bool IsInBattle;
    [ReadOnly] public bool IsDead;
    // �⺻������ ������ �� ���� ��ü�� ����մϴ�.
    [ReadOnly] public bool IsMovable;
    public bool IsInitialized;


    public EnemyStateMachine(Enemy enemy)
    {
        //Player = gameManager.Instance.Player;
        Enemy = enemy;
        Animator = enemy.Animator;
        IdleState = new EnemyIdleState(this);
        WanderState = new EnemyWanderState(this);
        ChaseState = new EnemyChaseState(this);
        AttackState = new EnemyAttackState(this);
        ReturnToBaseState = new EnemyReturnToBaseState(this);
        FleeState = new EnemyFleeState(this);
        DeadState = new EnemyDeadState(this);
        StunState = new EnemyStunState(this);
        IsFleeable = enemy.Data.IsFleeable;
        actionData = enemy.Actions;
        actionsToExecute = new List<AttackAction>(actionData.Length);
        IsPauseChanged += PauseAnimation;
        forceReceiver = enemy.ForceReceiver;
        agent = enemy.Agent;
        HP = enemy.Data.MaxHP;
        InitializeAffectedAttackEffectInfo();
        OnDie += Dead;
        Player = GameManager.Instance.Player;
        PositionableTarget = Player.GetComponent<Player>();
        InitializeMovable(enemy.Data.IsMovable);
        enemy.AnimEventReceiver.AnimEventCalled += EventDecision;
        IsInitialized = true;
    }

    private void EventDecision(AnimationEvent animEvent)
    {
        if (animEvent.stringParameter == "SetMovable")
        {
            SetMovable(animEvent.intParameter != 0);
        }
    }

    private void InitializeAffectedAttackEffectInfo()
    {
        AttackEffectTypes[] effectTypes = Enemy.Data.AffectedEffects;
        for (int i = 0; i < effectTypes.Length; i++)
        {
            affectedEffectInfo.SetFlag(effectTypes[i], true);
        }
    }
    public override void Update()
    {
        CheckTargetDistance();
        if (!isActive)
            return;
        if (IsInBattle)
            BattleTime += Time.deltaTime;
        MoveByForce();
        UpdateActivatedActions();
        base.Update();
    }
    public override void PhysicsUpdate()
    {
        if (!isActive)
            return;
        base.PhysicsUpdate();
    }
    private void CheckTargetDistance()
    {
        if (Time.time - lastCheckTime > ActivationCheckDelay)
        {
            CalculateTargetDistance();
            if (TargetDistance <= ActivationDistance)
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }
            lastCheckTime = Time.time;
        }
        else
        {
            if (isActive)
                CalculateTargetDistance();
        }
    }
    private void CalculateTargetDistance()
    {
        TargetDistance = Vector3.Distance(Player.transform.position, Enemy.transform.position);
    }
    public bool GetIsPause() => isPause;
    public void SetIsPause(bool isPause)
    {
        if (this.isPause != isPause)
        {
            this.isPause = isPause;
            IsPauseChanged.Invoke(isPause);
        }
    }

    public void PauseAnimation(bool isPause)
    {
        if (isPause)
        {
            Animator.speed = 0f;
        }
        else
        {
            Animator.speed = 1f;
        }
    }
    /// <summary>
    /// �׼��� ������ CurrentAction���� �����մϴ�. ������ �� �ִ� �׼��� ���ٸ� false�� �����մϴ�.
    /// </summary>
    /// <returns></returns>
    public bool ChooseAction()
    {
        // ���డ�� �׼��� ���ٸ� Data���� ���డ���� �׼��� �߰��մϴ�.
        if (actionsToExecute.Count == 0)
        {
            for (int i = 0; i < actionData.Length; i++)
            {
                if (actionData[i].Condition.isEligible())
                    actionsToExecute.Add(actionData[i]);
            }
            if (actionsToExecute.Count == 0)
                return false;
        }

        int prioritySum = 0;
        int priority = 0;
        for (int i = 0; i < actionsToExecute.Count; i++)
        {
            priority = actionsToExecute[i].Condition.Priority;
            if (priority >= ActionCondition.determinePriority)
            {
                CurrentAction = actionsToExecute[i];
                ClearExecutableActions();
                return true;
            }
            prioritySum += priority;
        }
        int executePivot = UnityEngine.Random.Range(1, prioritySum);
        int currentPriority = 0;
        for (int i = 0; i < actionsToExecute.Count; i++)
        {
            currentPriority += actionsToExecute[i].Condition.Priority;
            if (executePivot <= currentPriority)
            {
                CurrentAction = actionsToExecute[i];
                ClearExecutableActions();
                return true;
            }
        }
        Debug.LogError("�׼��� ���õ��� ���߽��ϴ�.");
        return false;
    }
    private void ClearExecutableActions()
    {
        for (int i = 0; i < actionsToExecute.Count; i++)
        {
            if (actionsToExecute[i].Condition.WaitUntilExecute)
                continue;
            actionsToExecute.RemoveAt(i);
            i--;
        }
    }
    private void UpdateActivatedActions()
    {
        for (int i = 0; i < actionsInActive.Count; i++)
        {
            actionsInActive[i].OnUpdate();
        }
    }
    public List<AttackAction> GetActionsInActive()
    {
        return actionsInActive;
    }
    public void AddActionInActive(AttackAction action)
    {
        actionsInActive.Add(action);
        ActivatedActionsChanged?.Invoke();
    }
    public void RemoveActionInActive(AttackAction action)
    {
        actionsInActive.Remove(action);
        ActivatedActionsChanged?.Invoke();
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (IsInvincible)
            return;
        if (!CanTakeDamageAndEffect(attacker))
            return;
        HP -= damage;
        Debug.Log("�� ����" + damage);
        HP = Mathf.Max(HP, 0);
        if (HP == 0)
        {
            OnDie?.Invoke();
            OnDieAction?.Invoke(Enemy);
        }

    }
    private void Dead()
    {
        actionsInActive.Clear();
        actionsToExecute.Clear();
        CurrentAction = null;
        IsDead = true;
        ServeQuestManager.Instance.QuestMonsterCheck(Enemy);
        ChangeState(DeadState);
    }

    public void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    {
        if (IsInvincible)
            return;
        if (!CanTakeDamageAndEffect(attacker))
            return;
        if (!AffectedEffectInfo.CanBeAffected(attackEffectTypes))
            return;

        switch (attackEffectTypes)
        {
            case AttackEffectTypes.KnockBack:
                Vector3 direction = Enemy.transform.position - attacker.transform.position;
                direction.Normalize();
                forceReceiver.AddForce(direction * value);
                break;
            case AttackEffectTypes.Airborne:
                forceReceiver.AddForce(Vector3.up * value);
                break;
            case AttackEffectTypes.Stun:
                StunState.SetStunTime(value);
                if (CurrentAction != null)
                    CurrentAction.Interrupt();
                CurrentAction = null;
                ChangeState(StunState);
                break;
        }
    }
    private void MoveByForce()
    {
        agent.Move(forceReceiver.Movement * Time.deltaTime);
    }
    private void SetMovable(bool isMovable)
    {
        IsMovable = isMovable;
        if (isMovable)
        {
            agent.isStopped = false;
        }
        else
        {
            agent.isStopped = true;
        }
    }
    private void InitializeMovable(bool isMovable)
    {
        IsMovable = isMovable;
    }
    public void ResetStateMachine()
    {
        HP = Enemy.Data.MaxHP;
        isActive = false;
        isPause = false;
        IsDead = false;
        IsInBattle = false;
        IsInvincible = false;
        IsFleeable = Enemy.Data.IsFleeable;
        BattleTime = 0f;
        OriginPosition = Vector3.zero;
        CurrentAction = null;
        ChangeState(IdleState);
    }
    private bool CanTakeDamageAndEffect(GameObject attacker)
    {
        CanBeAttackedTypes type = CanBeAttackedTypes.None;
        if (attacker.CompareTag(TagsAndLayers.PlayerTag))
            type = CanBeAttackedTypes.Player;
        else if (attacker.CompareTag(TagsAndLayers.EnemyTag))
            type = CanBeAttackedTypes.Enemy;
        else if (attacker.CompareTag(TagsAndLayers.EnvironmentTag))
            type = CanBeAttackedTypes.Environment;
        if (Enemy == attacker)
            return false;
        foreach (var characterType in Enemy.Data.CanBeAttackedType)
        {
            if (characterType == type)
                return true;
        }
        return false;
    }
}
