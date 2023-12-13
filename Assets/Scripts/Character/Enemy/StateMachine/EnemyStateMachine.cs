using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class EnemyStateMachine : StateMachine, IDamageable
{
    public event Action<bool> IsPauseChanged;
    public event Action ActivatedActionsChanged;
    public event Action OnDie;
    public event Action<Enemy> OnDieAction;
    public event Action<float> HpUpdated;
    public event Action ReleaseMonsterUI;

    // 게임 매니저에서 플레이어 불러옴.
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
    [SerializeField] private float activationCheckDelay = 0.5f;
    [SerializeField] private float activationDistance = 100f;
    public static readonly float monsterUIOnDistance = 30f;
    private bool isMonsterUIOn;
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
    // 기본적으로 움직일 수 없는 개체만 사용합니다.
    [ReadOnly] public bool IsMovable;
    public bool IsInitialized;
    private bool isMoveByForce;
    private Rigidbody rigidbody;
    private float verticalMovement;
    private bool isBattleBGMOn;


    public EnemyStateMachine(Enemy enemy)
    {
        //Player = gameManager.Instance.Player;
        Enemy = enemy;
        Animator = enemy.Animator;
        rigidbody = enemy.Rigidbody;
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
        OriginPosition = enemy.transform.position;
        enemy.AnimEventReceiver.AnimEventCalled += EventDecision;
        CalculateTargetDistance();
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
        SetMonsterUIByDistance();
        PlayBossBattleBGM();
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
        if (Time.time - lastCheckTime > activationCheckDelay)
        {
            CalculateTargetDistance();
            if (TargetDistance <= activationDistance && !isActive)
            {
                isActive = true;
                SetActive(true);
            }
            else if (TargetDistance > activationDistance && isActive)
            {
                isActive = false;
                SetActive(false);
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
    /// 액션을 선택해 CurrentAction으로 설정합니다. 선택할 수 있는 액션이 없다면 false를 리턴합니다.
    /// </summary>
    /// <returns></returns>
    public bool ChooseAction()
    {
        // 실행가능 액션이 없다면 Data에서 실행가능한 액션을 추가합니다.
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
        Debug.LogError("액션이 선택되지 못했습니다.");
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
        {
            if (attacker.CompareTag(TagsAndLayers.PlayerTag))
            {
                PlaySFX("OnHitBlocked", soundValue: 0.8f);
            }
            return;
        }
        PlaySFX("OnHit", 1.05f, 0.5f);
        HP -= damage;
        HP = Mathf.Max(HP, 0);
        HpUpdated?.Invoke((float)HP / Enemy.Data.MaxHP);
        if (HP == 0 && !IsDead)
        {
            if (attacker.CompareTag(TagsAndLayers.PlayerTag))
                DropReward();
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
        Enemy.Collider.enabled = false;
        Enemy.enabled = false;
        Enemy.ForceReceiver.enabled = false;
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
                direction.y = 0;
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
        if (forceReceiver.Movement == Vector3.zero)
        {
            if (isMoveByForce)
            {
                isMoveByForce = false;
                agent.enabled = true;
                rigidbody.useGravity = false;
                verticalMovement = 0f;
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            return;
        }
        else
        {
            bool isInNavMesh = NavMesh.SamplePosition(Enemy.transform.position + forceReceiver.Movement * Time.deltaTime, out NavMeshHit navHit, forceReceiver.Movement.magnitude * Time.deltaTime * 0.5f, agent.areaMask - (1 << NavMesh.GetAreaFromName("Walkable")));
            if (isInNavMesh && !isMoveByForce)
            {
                agent.Move(forceReceiver.Movement * Time.deltaTime);
            }
            else
            {
                isMoveByForce = true;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                rigidbody.drag = 0.1f;
                rigidbody.useGravity = true;
                agent.enabled = false;
                rigidbody.AddForce(forceReceiver.Movement);
            }
        }
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
        HpUpdated?.Invoke((float)HP / Enemy.Data.MaxHP);
        isActive = false;
        isPause = false;
        IsDead = false;
        IsInBattle = false;
        IsInvincible = false;
        IsFleeable = Enemy.Data.IsFleeable;
        Enemy.ForceReceiver.enabled = true;
        BattleTime = 0f;
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
    private void SetMonsterUIByDistance()
    {
        if (!isMonsterUIOn && TargetDistance <= monsterUIOnDistance && IsInitialized)
        {
            isMonsterUIOn = true;
            UI_Manager.Instance.CallEnemyHPBarUITurnOnEvent(Enemy);
        }
        else if (isMonsterUIOn && TargetDistance > monsterUIOnDistance)
        {
            isMonsterUIOn = false;
            ReleaseMonsterUI?.Invoke();
        }
    }
    private void PlayBossBattleBGM()
    {
        if (Enemy.Data.ID > 100)
            return;
        if (IsInBattle && !isBattleBGMOn)
        {
            isBattleBGMOn = true;
            SoundManager.Instance.BGMStop();
            if (Enemy.Data.ID == 5)
                SoundManager.Instance.ChangeBGM("영광의 권세");
            else
            {
                SoundManager.Instance.ChangeBGM("Boss");
            }
        }
        else if (!IsInBattle && isBattleBGMOn)
        {
            isBattleBGMOn = false;
            if (Enemy.Data.ID <= 100)
                SoundManager.Instance.PlayPreviousBGM();
        }
    }
    private void PlaySFX(string sfxFileName, float pitch = 1f, float soundValue = 1f)
    {
        SoundManager.Instance.CallPlaySFX(ClipType.EnemySFX, sfxFileName, Enemy.transform, false, pitch ,soundValue);
    }
    private void SetActive(bool isActive)
    {
        if (isActive)
        {
            Enemy.Collider.enabled = true;
            Enemy.Animator.enabled = true;
        }
        else
        {
            Enemy.Collider.enabled = false;
            Enemy.Animator.enabled = false;
        }
    }
    private void DropReward()
    {
        if (Enemy.Data.ID > 100)
            TradingManager.Instance.addMoney(200);
        else
            TradingManager.Instance.addMoney(2000);
        SoundManager.Instance.CallPlaySFX(ClipType.EnvironmentSFX, "pick", Enemy.transform, false, soundValue: 0.05f);
    }
}
