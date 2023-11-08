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
        agent.ResetPath();
        StartAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
        idleStartTime = Time.time;
        wanderWaitingTime = Random.Range(enemyData.MinWanderWaitTime, enemyData.MaxWanderWaitTime);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
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