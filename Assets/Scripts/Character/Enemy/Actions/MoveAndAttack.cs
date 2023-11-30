using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "MoveAndAttack", menuName = "Characters/Enemy/AttackAction/MoveAndAttack")]
public class MoveAndAttack : AttackAction
{
    private Transform targetTransform;
    private NavMeshAgent agent;
    private ObstacleAvoidanceType initialObstacleAvoidanceType;
    private Vector3 destination;
    private bool isMoving;
    
    public override void OnAwake()
    {
        base.OnAwake();
        targetTransform = StateMachine.Player.transform;
        agent = StateMachine.Enemy.Agent;
        initialObstacleAvoidanceType = agent.obstacleAvoidanceType;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }
    public override void OnStart()
    {
        base.OnStart();
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled += AnimationEventDecision;
    }
    public override void OnEnd()
    {
        base.OnEnd();
        agent.obstacleAvoidanceType = initialObstacleAvoidanceType;
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled -= AnimationEventDecision;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (isMoving)
        {
            Move();
        }
    }
    private void AnimationEventDecision(AnimationEvent animEvent)
    {
        if (animEvent.stringParameter == "Move")
        {
            isMoving = true;
        }
    }

    private void Move()
    {
        destination = targetTransform.position;
        Vector3 direction = destination - agent.transform.position;
        direction.y = 0;
        agent.Move(direction * agent.speed * Time.deltaTime);
    }
}
