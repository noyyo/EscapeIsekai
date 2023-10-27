using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        agent.speed = enemyData.RunSpeed * stateMachine.MovementSpeedModifier;
        StartAnimation(enemy.AnimationData.RunParameterHash);
    }
    public override void Exit()
    {
        base.Exit();
        StopAnimation(enemy.AnimationData.RunParameterHash);
    }
    public override void Update()
    {
        base.Update();
        if (IsInChaseRange())
        {
            if (IsInAttackRange())
            {
                stateMachine.ChangeState(stateMachine.AttackState);
            }
            else
            {
                Chase();
            }
        }
    }
    private void Chase()
    {
        NavMeshHit hit;
        Vector3 currentPosition = agent.transform.position;
        currentPosition.y -= agent.baseOffset;
        bool isInNavMesh = NavMesh.SamplePosition(currentPosition, out hit, 1f, 1 << NavMesh.GetAreaFromName("MonsterZone"));
        if (!isInNavMesh)
        {
            stateMachine.ChangeState(stateMachine.ReturnToBaseState);
        }
        else
        {
            agent.SetDestination(stateMachine.Player.transform.position);
        }
    }
}
