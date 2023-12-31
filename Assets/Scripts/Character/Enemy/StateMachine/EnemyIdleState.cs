using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private float idleStartTime;
    private float wanderWaitingTime;
    public EnemyIdleState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (enemyData.ID <= 100)
        stateMachine.IsInBattle = false;
        stateMachine.BattleTime = 0f;
        if (agent.enabled && stateMachine.IsMovable && stateMachine.IsInitialized)
        {
            agent.ResetPath();
        }
        StartAnimation(enemy.AnimationData.PeaceParameterHash);
        StartAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
        idleStartTime = Time.time;
        wanderWaitingTime = Random.Range(enemyData.MinWanderWaitTime, enemyData.MaxWanderWaitTime);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
        StopAnimation(enemy.AnimationData.PeaceParameterHash);
    }

    public override void Update()
    {
        base.Update();
        if (stateMachine.IsInStateTransition)
            return;
        if (stateMachine.CurrentState != this)
            return;
        if (IsInChaseRange())
        {
            ChangeBattleStance(true);
        }
        else if (Time.time - idleStartTime > wanderWaitingTime && !isStanceChanging)
        {
            stateMachine.ChangeState(stateMachine.WanderState);
            return;
        }
    }
}