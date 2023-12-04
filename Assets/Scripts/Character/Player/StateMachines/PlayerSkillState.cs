using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : PlayerGroundState
{

    protected HashSet<GameObject> alreadyCollided = new HashSet<GameObject>();

    public PlayerSkillState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isMovable = false;
        isStateChangeable = false;
        StartAnimation(stateMachine.Player.AnimationData.SkillParameterHash);
        stateMachine.Player.Playerconditions.UseSkill(groundData.SkillCost);
    }

    public override void Exit()
    {
        base.Exit();
        isStateChangeable = true;

        StopAnimation(stateMachine.Player.AnimationData.SkillParameterHash);
    }

    public override void Update()
    {
        base.Update();

        // �ִϸ��̼� �̸��� "Skill"�̰� �ִϸ��̼��� ������ �� ���¸� ����
        if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill") &&
            stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public void ApplyAttack(Collider other)
    {
        if (alreadyCollided.Contains(other.gameObject))
            return;

        alreadyCollided.Add(other.gameObject);
        IDamageable target = null;
        if (other.tag == TagsAndLayers.EnemyTag)
        {
            Enemy enemy;
            other.TryGetComponent(out enemy);
            if (enemy == null)
            {
                Debug.LogError("������ Enemy������Ʈ�� �����ϴ�.");
                return;
            }
            target = enemy.StateMachine;

        }
        else if (other.tag == TagsAndLayers.EnvironmentTag)
        {
            BaseEnvironmentObject environmentObj;
            other.TryGetComponent(out environmentObj);
            if (environmentObj == null)
            {
                Debug.LogError("��󿡰� BaseEnvironmentObject ������Ʈ�� �����ϴ�.");
                return;
            }
            target = environmentObj;
        }

        target?.TakeDamage(10 + stateMachine.Player.Playerconditions.Power);
    }
}
