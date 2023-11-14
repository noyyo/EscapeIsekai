using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class EnemyFleeState : EnemyBaseState
{
    private static readonly float safeDistance = 10f;
    private float lastLoactionUpdateTime;
    private float fleeLocationUpdateDelay = 0.4f;
    private Vector3 Direction;
    public EnemyFleeState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.IsInBattle = false;
        stateMachine.BattleTime = 0f;
        agent.speed = enemyData.RunSpeed * stateMachine.MovementSpeedModifier;
        StartAnimation(enemy.AnimationData.MoveParameterHash);
        StartAnimation(enemy.AnimationData.RunParameterHash);
    }
    public override void Exit()
    {
        base.Exit();
        StopAnimation(enemy.AnimationData.RunParameterHash);
        StopAnimation(enemy.AnimationData.MoveParameterHash);
    }
    public override void Update()
    {
        base.Update();
        if (Time.time - lastLoactionUpdateTime > fleeLocationUpdateDelay)
        {
            lastLoactionUpdateTime = Time.time;
            agent.SetDestination(GetFleeLocation());
        }
    }
    private Vector3 GetFleeLocation()
    {
        Direction = agent.transform.position - stateMachine.Player.transform.position;
        Direction.y = 0;
        NavMeshHit hit;
        NavMesh.SamplePosition(Direction.normalized * safeDistance, out hit, safeDistance, NavMesh.GetAreaFromName("MonsterZone"));
        return hit.position;
    }

}
