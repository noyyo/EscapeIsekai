using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

[Serializable]
public class EnemyStateMachine : StateMachine
{
    public event Action<bool> IsPauseChanged;
    public event Action ActivatedActionsChanged;
    // 게임 매니저에서 플레이어 불러옴.
    public GameObject Player { get; }
    public Enemy Enemy { get; }
    public Animator Animator;
    public float MovementSpeedModifier { get; set; } = 1f;
    public EnemyIdleState IdleState { get; }
    public EnemyWanderState WanderState { get; }
    public EnemyChaseState ChaseState { get; }
    public EnemyAttackState AttackState { get; }
    public EnemyReturnToBaseState ReturnToBaseState { get; }
    public EnemyFleeState FleeState { get; }
    private bool isPause;
    private bool isActive;
    [SerializeField] private float ActivationCheckDelay = 0.5f;
    [SerializeField] private float ActivationDistance = 100f;
    private float lastCheckTime;
    public float TargetDistance { get; private set; }
    public Vector3 OriginPosition { get; set; }
    public bool IsInvincible { get; set; }
    public bool IsFleeable { get; set; }
    public AttackAction CurrentAction;
    private AttackAction[] actionData;
    private List<AttackAction> actionsInActive = new List<AttackAction>(5);
    private List<AttackAction> actionsToExecute = new List<AttackAction>(5);
    private List<int> eligibleActionIndex;
    public int HP;
    public float BattleTime;
    

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
        IsFleeable = enemy.Data.IsFleeable;
        actionData = enemy.Actions;
        eligibleActionIndex = new List<int>(actionData.Length);
        IsPauseChanged += PauseAnimation;
        //Test
        Player = Enemy.Player;
    }

    public override void Update()
    {
        CheckTargetDistance();
        if (!isActive)
            return;
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
        // TODO : 게임 매니저에서 플레이어 불러와서 거리계산.
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
    // 액션을 선택해 CurrentAction으로 설정합니다. 선택할 수 있는 액션이 없다면 false를 리턴합니다.
    public bool ChooseAction()
    {
        // 실행대기 액션이 없다면 Data에서 선택합니다.
        if (actionsToExecute.Count == 0)
        {
            for (int i = 0; i < actionData.Length; i++)
            {
                if (actionData[i].Condition.isEligible())
                    eligibleActionIndex.Add(i);
            }
            if (eligibleActionIndex.Count == 0)
                return false;
            AttackAction choosedAction = actionData[UnityEngine.Random.Range(0, eligibleActionIndex.Count)];
            CurrentAction = choosedAction;
            eligibleActionIndex.Clear();
            return true;
        }

        int prioritySum = 0;
        int priority = 0;
        for (int i = 0; i < actionsToExecute.Count; i++)
        {
            priority = actionsToExecute[i].Condition.Priority;
            if (priority >= ActionCondition.determinePriority)
            {
                CurrentAction = actionsToExecute[i];
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
                return true;
            }
        }
        Debug.LogError("액션이 선택되지 못했습니다.");
        return false;
    }
    private void UpdateActivatedActions()
    {
        foreach (AttackAction action in actionsInActive)
        {
            action.OnUpdate();
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
}
