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
}
