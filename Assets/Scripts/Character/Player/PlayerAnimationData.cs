using System;
using UnityEngine;

[Serializable]
public class PlayerAnimationData
{
    [SerializeField] private string groundParameterName = "@Ground";
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string walkParameterName = "Walk";
    [SerializeField] private string runParameterName = "Run";
    [SerializeField] private string rollParameterName = "Roll";
    [SerializeField] private string airParameterName = "@Air";
    [SerializeField] private string jumpParameterName = "Jump";
    [SerializeField] private string fallParameterName = "Fall";
    [SerializeField] private string attackParameterName = "@Attack";
    [SerializeField] private string comboAttackParameterName = "ComboAttack";
    [SerializeField] private string skillParameterName = "Skill";
    [SerializeField] private string powerUpParameterName = "PowerUp";
    [SerializeField] private string superJumpParameterName = "SuperJump";
    [SerializeField] private string throwParameterName = "Throw";
    [SerializeField] private string noStaminaParameterName = "NoStamina";
    [SerializeField] private string shieldParameterName = "Shield";

    [SerializeField] private string deadParameterName = "Dead";

    public int GroundParameterHash { get; private set; }
    public int IdleParameterHash { get; private set; }
    public int WalkParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }
    public int RollParameterHash { get; private set; }
    public int AirParameterHash { get; private set; }
    public int JumpParameterHash { get; private set; }
    public int FallParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int ComboAttackParameterHash { get; private set; }
    public int SkillParameterHash { get; private set; }
    public int PowerUpParameterHash { get; private set; }
    public int SuperJumpParameterHash { get; private set; }
    public int ThrowParameterHash { get; private set; }
    public int NoStaminaParameterHash { get; private set; }
    public int ShieldParameterHash { get; private set; }
    public int DeadParameterHash { get; private set; }

    public void Initialize()
    {
        GroundParameterHash = Animator.StringToHash(groundParameterName);
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        WalkParameterHash = Animator.StringToHash(walkParameterName);
        RunParameterHash = Animator.StringToHash(runParameterName);
        RollParameterHash = Animator.StringToHash(rollParameterName);
        AirParameterHash = Animator.StringToHash(airParameterName);
        JumpParameterHash = Animator.StringToHash(jumpParameterName);
        FallParameterHash = Animator.StringToHash(fallParameterName);
        AttackParameterHash = Animator.StringToHash(attackParameterName);
        ComboAttackParameterHash = Animator.StringToHash(comboAttackParameterName);
        SkillParameterHash = Animator.StringToHash(skillParameterName);
        PowerUpParameterHash = Animator.StringToHash(powerUpParameterName);
        SuperJumpParameterHash = Animator.StringToHash(superJumpParameterName);
        ThrowParameterHash = Animator.StringToHash(throwParameterName);
        NoStaminaParameterHash = Animator.StringToHash(noStaminaParameterName);
        ShieldParameterHash = Animator.StringToHash(shieldParameterName);
        DeadParameterHash = Animator.StringToHash(deadParameterName);
    }
}
