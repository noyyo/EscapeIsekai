using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
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
    private List<AttackAction> actionsInActive;
    private List<AttackAction> actionsToExecute;
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
    private void ChooseAction()
    {
        // TODO : 예약 리스트 있으면 거기서 선택 없으면 ActionData에서 선택.
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
