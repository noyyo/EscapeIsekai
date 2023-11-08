using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }

    // States
    public PlayerIdleState IdleState { get; }
    public PlayerWalkState WalkState { get; }
    public PlayerRunState RunState { get; }
    public PlayerJumpState JumpState { get; }
    public PlayerRollState RollState { get; }
    public PlayerFallState FallState { get; }

    public PlayerComboAttackState ComboAttackState { get; }

    public PlayerSkillState SkillState { get; }
    public PlayerPowerUpState PowerUpState { get; }
    public Vector2 MovementInput { get; set; }
    public float MovementSpeed { get; set; }
    public float RotationDamping { get; private set; }
    public float MovementSpeedModifier { get; set; } = 1f;
    public float AttackPowerModifier { get; set; } = 1f;

    public float JumpForce { get; set; }
    public bool IsAttacking { get; set; }
    public int ComboIndex { get; set; }

    public List<Buff> buffs = new List<Buff>();

    private float checkDelay = 0.1f;
    private float lastCheckTime;

    public Transform MainCameraTransform { get; set; }

    public PlayerStateMachine(Player player)
    {
        this.Player = player;
        // this는 인스턴스이다.
        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        RunState = new PlayerRunState(this);
        RollState = new PlayerRollState(this);

        JumpState = new PlayerJumpState(this);
        FallState = new PlayerFallState(this);

        ComboAttackState = new PlayerComboAttackState(this);

        SkillState = new PlayerSkillState(this);
        PowerUpState = new PlayerPowerUpState(this);

        MainCameraTransform = Camera.main.transform;

        // 플레이어의 기본 데이터를 가져옴
        MovementSpeed = player.Data.GroundedData.WalkSpeed;
        RotationDamping = player.Data.GroundedData.BaseRotatingDamping;
    }


    public override void Update()
    {
        base.Update();
        
        if(Time.time - lastCheckTime >= checkDelay)
        {
            CheckBuff();
            lastCheckTime = Time.time;
        }
    }


    private void CheckBuff()
    {
        Buff buff;
        for (int i = 0; i < buffs.Count; i++)
        {
            buff = buffs[i];
            if(Time.time - buff.buffStartTime >= buff.Duration)
            {
                buff.EndBuff();
                buffs.Remove(buff);
                i--;
            }
        }
    }


    private void TakeEffect(AttackEffectTypes type, float value)
    {
        switch(type)
        {
            case AttackEffectTypes.KnockBack:
                
                break;
        }
    }
}
