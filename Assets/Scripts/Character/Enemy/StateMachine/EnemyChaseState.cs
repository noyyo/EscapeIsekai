using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class EnemyChaseState : EnemyBaseState
{
    private bool isLookTarget;
    private bool isChoosed;
    private bool isMoving;
    public static readonly float ChaseTime = 3f;
    private static readonly float actionCoolDownWaitTime = 1f;
    private static readonly float actionExecutableTime = 3f;
    private float stateStartTime;
    private AttackAction action;
    public EnemyChaseState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.IsInBattle = true;
        isLookTarget = false;
        StartAnimation(enemy.AnimationData.BattleParameterHash);
        isChoosed = stateMachine.ChooseAction();
        stateStartTime = Time.time;
        if (!isChoosed)
        {
            agent.isStopped = true;
        }
        else
        {
            action = stateMachine.CurrentAction;
            agent.autoBraking = false;
            agent.speed = enemyData.RunSpeed * stateMachine.MovementSpeedModifier;
        }

    }
    public override void Exit()
    {
        base.Exit();

        StopAnimation(enemy.AnimationData.BattleParameterHash);
        if (!isChoosed)
        {
            agent.isStopped = false;
        }
        else
        {
            StopAnimation(enemy.AnimationData.RunParameterHash);
            agent.autoBraking = true;
        }
    }
    public override void Update()
    {
        base.Update();
        if (!isChoosed)
        {
            if (Time.time - stateStartTime >= actionCoolDownWaitTime)
            {
                stateMachine.ChangeState(stateMachine.ChaseState);
                return;
            }
        }
        else
        {
            if (Time.time - stateStartTime >= actionExecutableTime)
            {
                stateMachine.ChangeState(stateMachine.ChaseState);
                return;
            }
            
        }
        if (IsInChaseRange())
        {
            Chase();
        }
        else
        {
            stateMachine.ChangeState(stateMachine.ReturnToBaseState);
            return;
        }
    }
    private void Chase()
    {
        if (action.Condition.isSatisfyDistanceCondition())
        {
            LookTarget();
            if (isLookTarget)
            {
                stateMachine.ChangeState(stateMachine.AttackState);
                return;
            }
        }
        else
        {
            float targetDistance = stateMachine.TargetDistance;
            if (targetDistance < action.Condition.MoreThanThisDistance)
            {
                // 사용 가능 거리보다 현재 거리가 짧으면 액션을 다시 선택해서 체이싱합니다.
                stateMachine.ChangeState(stateMachine.ChaseState);
            }
            else if (targetDistance > action.Condition.LessThanThisDistance)
            {
                if (stateMachine.Enemy.Data.RunSpeed == 0)
                {
                    stateMachine.ChangeState(stateMachine.ChaseState);
                    return;
                }
                MoveToTarget();
            }
        }

    }
    private void CheckIsLookTarget()
    {
        Vector3 forward = agent.transform.forward;
        forward.y = 0;
        Vector3 targetDirection = stateMachine.Player.transform.position - agent.transform.position;
        targetDirection.y = 0;

        float angleDifference = Vector3.Angle(targetDirection, forward);
        if (angleDifference <= 0.5f)
        {
            isLookTarget = true;
        }
    }
    private void LookTarget()
    {
        if (isMoving)
        {
            isMoving = false;
            StopAnimation(enemy.AnimationData.RunParameterHash);
        }
        Vector3 targetDirection = stateMachine.Player.transform.position - agent.transform.position;
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, agent.angularSpeed * stateMachine.MovementSpeedModifier * Time.deltaTime);

        CheckIsLookTarget();
    }
    private void MoveToTarget()
    {
        if (!isMoving)
        {
            isMoving = true;
            StartAnimation(enemy.AnimationData.RunParameterHash);
            agent.speed = enemyData.RunSpeed;
        }
        NavMeshHit hit;
        Vector3 currentPosition = agent.transform.position;
        currentPosition.y -= agent.baseOffset;
        bool isInNavMesh = NavMesh.SamplePosition(currentPosition, out hit, 1f, agent.areaMask - (1 << NavMesh.GetAreaFromName("Walkable")));
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
