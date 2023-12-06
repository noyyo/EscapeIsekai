public class EnemyReturnToBaseState : EnemyBaseState
{
    private static readonly float returnSpeed = 2f;
    public EnemyReturnToBaseState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StopAnimation(enemy.AnimationData.BattleParameterHash);
        agent.SetDestination(stateMachine.OriginPosition);
        agent.speed = enemyData.RunSpeed * returnSpeed;
        stateMachine.IsInvincible = true;
        stateMachine.CurrentAction = null;
    }
    public override void Exit()
    {
        base.Exit();
        stateMachine.IsInBattle = false;
        stateMachine.BattleTime = 0f;
        StopAnimation(enemy.AnimationData.ReturnToBaseParameterHash);
        stateMachine.IsInvincible = false;

    }
    public override void Update()
    {
        base.Update();
        if (agent.remainingDistance < 0.2f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
