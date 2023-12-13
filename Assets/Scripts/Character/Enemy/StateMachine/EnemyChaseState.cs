using UnityEngine;
using UnityEngine.AI;

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
    private bool isChangingState;
    public EnemyChaseState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        isChangingState = false;
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
        if (!isChoosed && stateMachine.IsMovable && enemy.isActiveAndEnabled)
        {
            agent.isStopped = false;
        }
        else
        {
            StopAnimation(enemy.AnimationData.RunParameterHash);
            if (agent.isActiveAndEnabled)
                agent.autoBraking = true;
        }
    }
    public override void Update()
    {
        base.Update();
        if (isChangingState)
            return;
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
                if (isMoving)
                    StartAnimation(enemy.AnimationData.RunParameterHash);
                return;
            }

        }
        if (!agent.isActiveAndEnabled)
            return;
        if (IsInChaseRange())
        {
            Chase();
        }
        else
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            StopAnimation(enemy.AnimationData.RunParameterHash);
            ChangeBattleStance(false);
            isChangingState = true;
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
                agent.velocity = Vector3.zero;
                stateMachine.ChangeState(stateMachine.AttackState);
                isChangingState = true;
                return;
            }
        }
        else
        {
            float targetDistance = stateMachine.TargetDistance;
            if (targetDistance < action.Condition.MoreThanThisDistance)
            {
                // 사용 가능 거리보다 현재 거리가 짧으면 액션을 다시 선택해서 체이싱합니다.
                isChangingState = true;
                stateMachine.ChangeState(stateMachine.ChaseState);
                return;
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
        if (isMoving)
        {
            isMoving = false;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
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
        if (!stateMachine.IsMovable)
        {
            stateMachine.ChangeState(stateMachine.ChaseState);
            isChangingState = true;
            return;
        }
        if (!isMoving)
        {
            isMoving = true;
            StartAnimation(enemy.AnimationData.RunParameterHash);
            agent.speed = enemyData.RunSpeed;
        }
        NavMeshHit hit;
        Vector3 currentPosition = agent.transform.position;
        currentPosition.y += agent.baseOffset;
        bool isInNavMesh = NavMesh.SamplePosition(currentPosition, out hit, 1f, agent.areaMask - (1 << NavMesh.GetAreaFromName("Walkable")));
        if (!isInNavMesh)
        {
            StopAnimation(enemy.AnimationData.RunParameterHash);
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            ChangeBattleStance(false);
            isChangingState = true;
            isMoving = false;
        }
        else
        {
            if (NavMesh.SamplePosition(stateMachine.Player.transform.position, out hit, 5f, agent.areaMask - (1 << NavMesh.GetAreaFromName("Walkable"))))
                agent.SetDestination(hit.position);
            else
            {
                StopAnimation(enemy.AnimationData.RunParameterHash);
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                ChangeBattleStance(false);
                isChangingState = true;
                isMoving = false;
            }
        }
    }
}
