using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseState : IState
{
    public EnemyStateMachine stateMachine;
    public Enemy enemy;
    public EnemySO enemyData;
    public Animator animator;
    public Rigidbody rigidbody;
    public NavMeshAgent agent;
    public CharacterController controller;
    private static readonly string changeBattleStanceTag = "ChangeStance";
    private bool isAnimStarted;
    private bool isTargetStanceBattle;
    protected bool isStanceChanging;

    public EnemyBaseState(EnemyStateMachine enemyStateMachine)
    {
        stateMachine = enemyStateMachine;
        enemy = enemyStateMachine.Enemy;
        animator = enemy.Animator;
        rigidbody = enemyStateMachine.Enemy.Rigidbody;
        agent = enemyStateMachine.Enemy.Agent;
        enemyData = enemyStateMachine.Enemy.Data;
    }
    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void HandleInput()
    {

    }
    public virtual void Update()
    {
        if (stateMachine.GetIsPause())
            return;
        CheckStanceChanging();
    }
    public virtual void PhysicsUpdate()
    {
        if (stateMachine.GetIsPause())
            return;
    }
    public void StartAnimation(int animationHash)
    {
        animator.SetBool(animationHash, true);
    }

    public void StopAnimation(int animationHash)
    {
        animator.SetBool(animationHash, false);
    }
    protected bool IsInChaseRange()
    {
        return stateMachine.TargetDistance <= enemyData.ChasingRange;
    }
    protected void ChangeBattleStance(bool isTargetStanceBattle)
    {
        if (isStanceChanging)
            return;
        this.isTargetStanceBattle = isTargetStanceBattle;
        isStanceChanging = true;
        CheckStanceChanging();
    }
    private void CheckStanceChanging()
    {
        if (!isStanceChanging)
            return;
        if (isTargetStanceBattle)
        {
            StartAnimation(enemy.AnimationData.BattleParameterHash);
            AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!isAnimStarted && currentInfo.IsTag(changeBattleStanceTag))
            {
                isAnimStarted = true;
            }
            else if (isAnimStarted && !currentInfo.IsTag(changeBattleStanceTag))
            {
                isAnimStarted = false;
                isStanceChanging = false;
                stateMachine.ChangeState(stateMachine.ChaseState);
            }
        }
        else
        {
            StartAnimation(enemy.AnimationData.PeaceParameterHash);
            AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!isAnimStarted && currentInfo.IsTag(changeBattleStanceTag))
            {
                StartAnimation(enemy.AnimationData.ReturnToBaseParameterHash);
                isAnimStarted = true;
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }
            else if (isAnimStarted && !currentInfo.IsTag(changeBattleStanceTag))
            {
                isAnimStarted = false;
                isStanceChanging = false;
                if (stateMachine.IsMovable)
                    agent.isStopped = false;
                stateMachine.ChangeState(stateMachine.ReturnToBaseState);
            }
        }
    }
}
