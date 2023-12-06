using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : PlayerGroundState
{
    private float normalizedTime;

    protected HashSet<GameObject> alreadyCollided = new HashSet<GameObject>();

    public PlayerSkillState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isMovable = false;
        StartAnimation(stateMachine.Player.AnimationData.SkillParameterHash);
        stateMachine.Player.Playerconditions.UseSkill(groundData.SkillCost);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "Skill", stateMachine.Player.transform, false);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.SkillParameterHash);
        soundManager.CallStopLoopSFX(ClipType.PlayerSFX, "Skill");
    }

    public override void Update()
    {
        base.Update();

        normalizedTime = GetNormalizedTime("Skill");
        if (normalizedTime >= 1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
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
                Debug.LogError("적에게 Enemy컴포넌트가 없습니다.");
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
                Debug.LogError("대상에게 BaseEnvironmentObject 컴포넌트가 없습니다.");
                return;
            }
            target = environmentObj;
        }

        target?.TakeDamage(10 + stateMachine.Player.Playerconditions.Power, stateMachine.Player.gameObject);
    }
}
