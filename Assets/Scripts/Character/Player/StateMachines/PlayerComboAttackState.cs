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

        // 초기화
        alreadyApplyCombo = false;
        alreadyAppliedForce = false;

        // 스테이트머신에서 콤보인덱스를 가져오고
        // 현재 사용해야 하는 콤보인덱스의 정보를 가져오고
        // 콤보인덱스 적용(해시값 가져오지 않고)
        // 공격에 대한 정보를 가져오고 그에 대한 인덱스를 적용시키는 로직
        int comboIndex = stateMachine.ComboIndex;
        attackInfoData = stateMachine.Player.Data.AttackData.GetAttackInfo(comboIndex);
        stateMachine.Player.Animator.SetInteger("Combo", comboIndex);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.ComboAttackParameterHash);
        alreadyCollided.Clear();

        // 콤보가 끊겼을 때 다시 인덱스값을 0으로 돌림
        if (!alreadyApplyCombo)
            stateMachine.ComboIndex = 0;
    }

    private void TryComboAttack()
    {
        // 콤보어택을 이미 했다면 리턴
        if (alreadyApplyCombo) return;
        // -1은 마지막3타이므로 마지막 공격을 했다면 리턴
        if (attackInfoData.ComboStateIndex == -1) return;
        // 공격이 끊겼다면 리턴
        if (!stateMachine.IsAttacking) return;
        // 위의 모든 조건이 아니라면 true로 적용
        alreadyApplyCombo = true;
    }

    private void TryApplyForce()
    {
        // 이미 적용한 적이 있으면 리턴 그렇지 않다면 true
        if (alreadyAppliedForce) return;
        alreadyAppliedForce = true;
        // 받고있던 힘을(Force) 리셋하고
        stateMachine.Player.ForceReceiver.Reset();
        // ForceReceiver에 AddForce를 적용. 바라보고있는 정면에서 밀려나도록.
        stateMachine.Player.ForceReceiver.AddForce(stateMachine.Player.transform.forward * attackInfoData.Force);
    }

    public override void Update()
    {
        base.Update();

        ForceMove();
        // Animator를 전달하고 "Attack"이라는 태그를 주면 가져올 수 있다
        float normalizedTime = GetNormalizedTime("Attack");
        if (normalizedTime < 1f)
        {
            // 애니메이션이 처리가 되고 있는 중
            if (normalizedTime >= attackInfoData.ForceTransitionTime)
                TryApplyForce();

            if (normalizedTime >= attackInfoData.ComboTransitionTime)
                TryComboAttack();
        }
        else
        {
            // 애니메이션 플레이가 완료된 시점
            // 다음 콤보 인덱스를 가져와서 콤보어택을 시전하는 로직
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

        target?.TakeDamage(attackInfoData.Damage + stateMachine.Player.Playerconditions.Power);
        target?.TakeEffect(attackInfoData.AttackEffectType, attackInfoData.AttackEffectValue, stateMachine.Player.gameObject);
    }
}
