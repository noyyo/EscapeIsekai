using UnityEngine;
using UnityEngine.AI;

public class EnemyFleeState : EnemyBaseState
{
    private static readonly float safeDistance = 10f;
    private float lastLocationUpdateTime;
    private float fleeLocationUpdateDelay = 0.4f;
    private Vector3 Direction;
    public EnemyFleeState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
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
        if (Time.time - lastLocationUpdateTime > fleeLocationUpdateDelay)
        {
            lastLocationUpdateTime = Time.time;
            agent.SetDestination(GetFleeLocation());
        }
    }
    private Vector3 GetFleeLocation()
    {
        Direction = agent.transform.position - stateMachine.Player.transform.position;
        Direction.y = 0;
        NavMeshHit hit;
        NavMesh.SamplePosition(Direction.normalized * safeDistance, out hit, safeDistance, agent.areaMask - NavMesh.GetAreaFromName("Walkable"));
        return hit.position;
    }

}
