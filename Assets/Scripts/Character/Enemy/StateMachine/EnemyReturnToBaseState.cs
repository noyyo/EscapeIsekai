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
        StartAnimation(enemy.AnimationData.MoveParameterHash);
        stateMachine.IsInBattle = false;
        stateMachine.BattleTime = 0f;
        agent.SetDestination(stateMachine.OriginPosition);
        agent.speed = enemyData.RunSpeed * returnSpeed;
        stateMachine.IsInvincible = true;
        StartAnimation(enemy.AnimationData.RunParameterHash);
        stateMachine.CurrentAction = null;
    }
    public override void Exit()
    {
        base.Exit();
        StopAnimation(enemy.AnimationData.RunParameterHash);
        StopAnimation(enemy.AnimationData.MoveParameterHash);
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
