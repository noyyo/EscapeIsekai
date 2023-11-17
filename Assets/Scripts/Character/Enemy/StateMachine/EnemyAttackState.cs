using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    AttackAction action;
    public EnemyAttackState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(enemy.AnimationData.BattleParameterHash);
        agent.ResetPath();
        action = stateMachine.CurrentAction;
        action.OnStart();
    }

    public override void Exit()
    {
        base.Exit();
        action.OnEnd();
        StopAnimation(enemy.AnimationData.BattleParameterHash);
    }

    public override void Update()
    {
        base.Update();
        action.OnUpdate();
    }
}