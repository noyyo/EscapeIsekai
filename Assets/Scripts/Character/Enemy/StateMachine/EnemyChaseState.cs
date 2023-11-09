using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class EnemyChaseState : EnemyBaseState
{
    private bool isLookTarget;
    private bool isChoosed;
    public static readonly float ChaseTime = 3f;
    private static readonly float actionCoolDownWaitTime = 1f;
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
        StartAnimation(enemy.AnimationData.MoveParameterHash);
        isChoosed = stateMachine.ChooseAction();
        if (!isChoosed)
        {
            stateStartTime = Time.time;
            agent.isStopped = true;
            StartAnimation(enemy.AnimationData.IdleParameterHash);
        }
        else
        {
            action = stateMachine.CurrentAction;
            agent.autoBraking = false;
            agent.speed = enemyData.RunSpeed * stateMachine.MovementSpeedModifier;
            StartAnimation(enemy.AnimationData.RunParameterHash);
        }

    }
    public override void Exit()
    {
        base.Exit();
        if (!isChoosed)
        {
            agent.isStopped = false;
            StopAnimation(enemy.AnimationData.IdleParameterHash);
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
                stateMachine.ChangeState(stateMachine.ChaseState);
            return;
        }
        if (IsInChaseRange())
        {
            Chase();
        }
        else
        {
            stateMachine.ChangeState(stateMachine.ReturnToBaseState);
        }
    }
    private void Chase()
    {
        if (action.Condition.isSatisfyDistanceCondition())
        {
            if (isLookTarget)
            {
                stateMachine.ChangeState(stateMachine.AttackState);
                return;
            }
            LookTarget();
        }
        else
        {
            float targetDistance = stateMachine.TargetDistance;
            if (targetDistance < action.Condition.MoreThanThisDistance)
            {
                // ��� ���� �Ÿ����� ���� �Ÿ��� ª���� �׼��� �ٽ� �����ؼ� ü�̽��մϴ�.
                stateMachine.ChangeState(stateMachine.ChaseState);
            }
            else if (targetDistance > action.Condition.LessThanThisDistance)
            {
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
        Vector3 targetDirection = stateMachine.Player.transform.position - agent.transform.position;
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, agent.angularSpeed * stateMachine.MovementSpeedModifier * Time.deltaTime);

        CheckIsLookTarget();
    }
    private void MoveToTarget()
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
        CheckIsLookTarget();
    }
}
