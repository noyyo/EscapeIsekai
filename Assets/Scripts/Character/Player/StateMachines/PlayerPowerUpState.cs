using UnityEngine;

public class PlayerPowerUpState : PlayerGroundState
{
    private float powerUpStartTime;
    private Buff buff;
    public PlayerPowerUpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.PowerUpParameterHash);
        stateMachine.Player.Playerconditions.ActivePowerUp(groundData.PowerUpCost);
        powerUpStartTime = Time.time;
        buff = new Buff(BuffTypes.speed, stateMachine);  // 버프타입의 객체를 새로 생성된 메모리 주소를 가짐.
        buff.ApplyBuff(10);
        stateMachine.buffs.Add(buff);
        isMovable = false;
        isStateChangeable = false;

    }


    public override void Update()
    {
        base.Update();


        // 애니메이션 이름이 "PowerUp"이고 애니메이션이 끝났을 때 상태를 변경
        if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("PowerUp") &&
            stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.PowerUpParameterHash);
        isStateChangeable = true;

    }



}
