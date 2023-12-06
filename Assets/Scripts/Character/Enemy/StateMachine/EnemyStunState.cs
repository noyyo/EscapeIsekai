using UnityEngine;

public class EnemyStunState : EnemyBaseState
{
    private float stunTime;
    private float stunStartTime;
    public EnemyStunState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        stunStartTime = Time.time;
        StartAnimation(enemy.AnimationData.StunParameterHash);
        StartAnimation(enemy.AnimationData.StunStartParameterHash);
        if (stateMachine.IsMovable)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }
    }
    public override void Exit()
    {
        base.Exit();
        StopAnimation(enemy.AnimationData.StunParameterHash);
        if (stateMachine.IsMovable)
        {
            agent.isStopped = false;
        }
    }
    public override void Update()
    {
        base.Update();
        if (Time.time - stunStartTime >= stunTime)
        {
            if (stateMachine.IsInBattle)
            {
                stateMachine.ChangeState(stateMachine.ChaseState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }
    }
    public void SetStunTime(float stunTime)
    {
        this.stunTime = stunTime;
    }
}
