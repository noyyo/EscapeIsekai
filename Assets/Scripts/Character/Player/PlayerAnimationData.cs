using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAnimationData
{
    // Animator ���� ����� Parameter ���� ����
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

    [SerializeField] private string SkillParameterName = "Skill";
    [SerializeField] private string PowerUpParameterName = "PowerUp";

    // Parameter���� �޴� getter ����
    // �ش� Parameter�� ũ�� �� ������ ����� ���(Ground, Air, Attack)
    // ���Ŀ� ���� ��ɵ��� ������ ���¿� �°� �߰�
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

        SkillParameterHash = Animator.StringToHash(SkillParameterName);
        PowerUpParameterHash = Animator.StringToHash(PowerUpParameterName);
    }
}
