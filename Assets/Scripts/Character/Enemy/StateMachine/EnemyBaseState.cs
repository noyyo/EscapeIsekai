using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
        animator = enemyStateMachine.Enemy.Animator;
        rigidbody = enemyStateMachine.Enemy.Rigidbody;
        agent = enemyStateMachine.Enemy.Agent;
        controller = enemyStateMachine.Enemy.Controller;
        enemyData = enemyStateMachine.Enemy.Data;
        stateMachine.IsPauseChanged += PauseAnimation;
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
    protected void StartAnimation(int animationHash)
    {
        animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        animator.SetBool(animationHash, false);
    }
    protected void PauseAnimation(bool isPause)
    {
        if (isPause)
        {
            animator.speed = 0f;
        }
        else
        {
            animator.speed = 1f;
        }
    }
    protected bool IsInChaseRange()
    {
        return stateMachine.TargetDistance <= enemyData.ChasingRange;
    }
    protected bool IsInAttackRange()
    {
        return stateMachine.TargetDistance <= enemyData.AttackRange;
    }
}
