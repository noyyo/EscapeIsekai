using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStateMachine : StateMachine, IDamageable
{
    public Player Player { get; }

    // States
    public PlayerIdleState IdleState { get; }
    public PlayerWalkState WalkState { get; }
    public PlayerRunState RunState { get; }
    public PlayerJumpState JumpState { get; }
    public PlayerRollState RollState { get; }
    public PlayerFallState FallState { get; }

    public PlayerNothingState NothingState { get; }
    public PlayerComboAttackState ComboAttackState { get; }

    public PlayerSkillState SkillState { get; }
    public PlayerPowerUpState PowerUpState { get; }

    public PlayerSuperJump SuperJump { get; }

    public PlayerThrowState ThrowState { get; }

    public PlayerNoStamina NoStamina { get; }

    public PlayerShieldState ShieldState { get; }
    public PlayerDeadState DeadState { get; }

    public Vector2 MovementInput { get; set; }
    public float MovementSpeed { get; set; }
    public float RotationDamping { get; private set; }
    public float MovementSpeedModifier { get; set; } = 1f;
    public float AttackPowerModifier { get; set; } = 1f;

    public float JumpForce { get; set; }
    public bool IsAttacking { get; set; }
    public int ComboIndex { get; set; }

    [HideInInspector] public List<Buff> buffs = new List<Buff>();

    private float checkDelay = 0.1f;
    private float lastCheckTime;

    private bool shieldActive = false;

    public event Action OnDie;

    public Transform MainCameraTransform { get; set; }
    private AffectedAttackEffectInfo affectedEffectInfo = new AffectedAttackEffectInfo();
    public AffectedAttackEffectInfo AffectedEffectInfo { get => affectedEffectInfo; }

    public PlayerStateMachine(Player player)
    {
        this.Player = player;
        // this�� �ν��Ͻ��̴�.
        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        RunState = new PlayerRunState(this);
        RollState = new PlayerRollState(this);

        JumpState = new PlayerJumpState(this);
        FallState = new PlayerFallState(this);
        NothingState = new PlayerNothingState(this);
        ComboAttackState = new PlayerComboAttackState(this);

        SkillState = new PlayerSkillState(this);
        PowerUpState = new PlayerPowerUpState(this);

        SuperJump = new PlayerSuperJump(this);
        ThrowState = new PlayerThrowState(this);
        NoStamina = new PlayerNoStamina(this);
        ShieldState = new PlayerShieldState(this);

        DeadState = new PlayerDeadState(this);

        MainCameraTransform = Camera.main.transform;

        // �÷��̾��� �⺻ �����͸� ������
        MovementSpeed = player.Data.GroundedData.WalkSpeed;
        RotationDamping = player.Data.GroundedData.BaseRotatingDamping;
        OnDie += Dead;
        AffectedEffectInfo.SetFlag(AffectedAttackEffectInfo.AllFlag);
        Player.Weapon.WeaponColliderEnter += OnWeaponColliderEnter;
    }


    public override void Update()
    {
        base.Update();

        if (Time.time - lastCheckTime >= checkDelay)
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
            if (Time.time - buff.buffStartTime >= buff.duration)
            {
                buff.EndBuff();
                buffs.Remove(buff);
                i--;
            }
        }
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (!shieldActive && CurrentState != RollState)
        {
            Player.Playerconditions.health.Subtract(damage - Player.Playerconditions.Guard);
        }
        else
        {
            return;
        }

        if (Player.Playerconditions.health.curValue <= 0f)
        {
            OnDie?.Invoke();
        }
    }

    public void ActivateShield()
    {
        shieldActive = true;
    }

    public void DeActivateShield()
    {
        shieldActive = false;
    }

    private void Dead()
    {
        // TODO : ���� �� ó���� ����
        ChangeState(DeadState);
    }

    public void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    {
        if (!AffectedEffectInfo.CanBeAffected(attackEffectTypes))
            return;
        Vector3 direction = Player.transform.position - attacker.transform.position;
        switch (attackEffectTypes)
        {
            case AttackEffectTypes.KnockBack:
                direction.Normalize();
                Player.ForceReceiver.AddForce(direction * value);
                // KnockBack ����
                break;
            case AttackEffectTypes.Airborne:
                Player.ForceReceiver.AddForce(Vector3.up * value);
                direction.y = 0;
                Player.ForceReceiver.AddForce(direction.normalized * value);
                // Airborne ����
                break;
            case AttackEffectTypes.Stun:
                // Stun ����
                break;
        }
    }
    private void OnWeaponColliderEnter(Collider other)
    {
        if (CurrentState != ComboAttackState && CurrentState != SkillState)
            return;

        if (CurrentState == ComboAttackState)
            ComboAttackState.ApplyAttack(other);
        else
            SkillState.ApplyAttack(other);
    }
}
