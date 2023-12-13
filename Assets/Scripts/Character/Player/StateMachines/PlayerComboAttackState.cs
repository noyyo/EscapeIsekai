using System.Collections.Generic;
using UnityEngine;

public class PlayerComboAttackState : PlayerAttackState
{
    private bool alreadyApplyCombo;
    private bool alreadyAppliedForce;

    AttackInfoData attackInfoData;

    protected HashSet<GameObject> alreadyCollided = new HashSet<GameObject>();
    public PlayerComboAttackState(PlayerStateMachine playerstateMachine) : base(playerstateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.ComboAttackParameterHash);
        soundManager.CallPlaySFX(ClipType.PlayerSFX, "Attack_Club", stateMachine.Player.transform, false, 1f, 0.05f);
        alreadyApplyCombo = false;
        alreadyAppliedForce = false;

        int comboIndex = stateMachine.ComboIndex;
        attackInfoData = stateMachine.Player.Data.AttackData.GetAttackInfo(comboIndex);
        stateMachine.Player.Animator.SetInteger("Combo", comboIndex);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.ComboAttackParameterHash);
        alreadyCollided.Clear();
        soundManager.CallStopLoopSFX(ClipType.PlayerSFX, "Attack_Club");

        if (!alreadyApplyCombo)
            stateMachine.ComboIndex = 0;
    }

    private void TryComboAttack()
    {
        if (alreadyApplyCombo) return;
        if (attackInfoData.ComboStateIndex == -1) return;
        if (!stateMachine.IsAttacking) return;
        alreadyApplyCombo = true;
    }

    private void TryApplyForce()
    {
        if (alreadyAppliedForce) return;
        alreadyAppliedForce = true;
        stateMachine.Player.ForceReceiver.Reset();
        stateMachine.Player.ForceReceiver.AddForce(stateMachine.Player.transform.forward * attackInfoData.Force);
    }

    public override void Update()
    {
        base.Update();
        ForceMove();
        float normalizedTime = GetNormalizedTimeforCombo(stateMachine.Player.Animator, "Attack");
        if (normalizedTime < 1f)
        {
            if (normalizedTime >= attackInfoData.ForceTransitionTime)
                TryApplyForce();

            if (normalizedTime >= attackInfoData.ComboTransitionTime)
                TryComboAttack();
        }
        else
        {
            if (alreadyApplyCombo)
            {
                stateMachine.ComboIndex = attackInfoData.ComboStateIndex;
                stateMachine.ChangeState(stateMachine.ComboAttackState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }
    }
    public void ApplyAttack(Collider other)
    {
        if (other.gameObject == stateMachine.Player)
            return;
        if (!(other.gameObject.layer == TagsAndLayers.CharacterLayerIndex || other.CompareTag(TagsAndLayers.EnvironmentTag) || other.gameObject.layer == TagsAndLayers.PlayerLayerIndex))
            return;
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
        target?.TakeDamage(attackInfoData.Damage + stateMachine.Player.Playerconditions.Power, stateMachine.Player.gameObject);
        target?.TakeEffect(attackInfoData.AttackEffectType, attackInfoData.AttackEffectValue, stateMachine.Player.gameObject);
    }
}
