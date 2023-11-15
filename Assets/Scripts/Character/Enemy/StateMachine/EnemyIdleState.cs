using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

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
        stateMachine.IsInBattle = false;
        stateMachine.BattleTime = 0f;
        if (agent.enabled)
            agent.ResetPath();
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
        if (IsInChaseRange())
        {
            stateMachine.ChangeState(stateMachine.ChaseState);
        }
        else if (Time.time - idleStartTime > wanderWaitingTime)
        {
            stateMachine.ChangeState(stateMachine.WanderState);
        }
    }
}