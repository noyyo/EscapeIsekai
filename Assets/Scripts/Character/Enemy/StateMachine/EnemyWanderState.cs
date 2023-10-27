using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWanderState : EnemyBaseState
{
    public EnemyWanderState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        agent.SetDestination(GetWanderLocation());
        agent.speed = enemyData.WalkSpeed * stateMachine.MovementSpeedModifier;
        StartAnimation(enemy.AnimationData.WalkParameterHash);
    }
    public override void Exit()
    {
        base.Exit();
        StopAnimation(enemy.AnimationData.WalkParameterHash);
    }
    public override void Update()
    {
        base.Update();
        if (agent.remainingDistance < 0.2f)
            stateMachine.ChangeState(stateMachine.IdleState);
    }
    private Vector3 GetWanderLocation()
    {
        Vector3 currentPosition = agent.transform.position;
        NavMeshHit hit = new NavMeshHit();
        float destinationDistance = 0f;
        int i = 0;
        while (destinationDistance < enemyData.MinWanderDistance)
        {
            NavMesh.SamplePosition(currentPosition + (OnUnitCircle() * Random.Range(enemyData.MinWanderDistance, enemyData.MaxWanderDistance)), out hit, enemyData.MaxWanderDistance, NavMesh.AllAreas);
            destinationDistance = Vector3.Distance(currentPosition, hit.position);
            // 과도한 샘플링 방지
            i++;
            if (i >= 20)
                break;
        }
        return hit.position;
    }
    private Vector3 OnUnitCircle()
    {
        int angle = Random.Range(0, 360);
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
