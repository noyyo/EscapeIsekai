using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReturnToBaseState : EnemyBaseState
{
    private static readonly float returnSpeed = 2f;
    public EnemyReturnToBaseState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        agent.SetDestination(stateMachine.OriginPosition);
        agent.speed = enemyData.RunSpeed * returnSpeed;
        stateMachine.IsInvincible = true;
        StartAnimation(enemy.AnimationData.RunParameterHash);
    }
    public override void Exit()
    {
        base.Exit();
        StopAnimation(enemy.AnimationData.RunParameterHash);
        stateMachine.IsInvincible = false;

    }
    public override void Update()
    {
        base.Update();
        if (agent.remainingDistance < 0.2f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
