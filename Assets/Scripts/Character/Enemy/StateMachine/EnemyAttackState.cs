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
        agent.ResetPath();
        action = stateMachine.CurrentAction;
        action.OnStart();
        StartAnimation(stateMachine.Enemy.AnimationData.AttackParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Enemy.AnimationData.AttackParameterHash);
        action.OnEnd();
    }

    public override void Update()
    {
        base.Update();
        action.OnUpdate();
    }
}