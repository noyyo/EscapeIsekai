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

        // �ʱ�ȭ
        alreadyApplyCombo = false;
        alreadyAppliedForce = false;

        // ������Ʈ�ӽſ��� �޺��ε����� ��������
        // ���� ����ؾ� �ϴ� �޺��ε����� ������ ��������
        // �޺��ε��� ����(�ؽð� �������� �ʰ�)
        // ���ݿ� ���� ������ �������� �׿� ���� �ε����� �����Ű�� ����
        int comboIndex = stateMachine.ComboIndex;
        attackInfoData = stateMachine.Player.Data.AttackData.GetAttackInfo(comboIndex);
        stateMachine.Player.Animator.SetInteger("Combo", comboIndex);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.ComboAttackParameterHash);
        alreadyCollided.Clear();

        // �޺��� ������ �� �ٽ� �ε������� 0���� ����
        if (!alreadyApplyCombo)
            stateMachine.ComboIndex = 0;
    }

    private void TryComboAttack()
    {
        // �޺������� �̹� �ߴٸ� ����
        if (alreadyApplyCombo) return;
        // -1�� ������3Ÿ�̹Ƿ� ������ ������ �ߴٸ� ����
        if (attackInfoData.ComboStateIndex == -1) return;
        // ������ ����ٸ� ����
        if (!stateMachine.IsAttacking) return;
        // ���� ��� ������ �ƴ϶�� true�� ����
        alreadyApplyCombo = true;
    }

    private void TryApplyForce()
    {
        // �̹� ������ ���� ������ ���� �׷��� �ʴٸ� true
        if (alreadyAppliedForce) return;
        alreadyAppliedForce = true;
        // �ް��ִ� ����(Force) �����ϰ�
        stateMachine.Player.ForceReceiver.Reset();
        // ForceReceiver�� AddForce�� ����. �ٶ󺸰��ִ� ���鿡�� �з�������.
        stateMachine.Player.ForceReceiver.AddForce(stateMachine.Player.transform.forward * attackInfoData.Force);
    }

    public override void Update()
    {
        base.Update();

        ForceMove();
        // Animator�� �����ϰ� "Attack"�̶�� �±׸� �ָ� ������ �� �ִ�
        float normalizedTime = GetNormalizedTime("Attack");
        if (normalizedTime < 1f)
        {
            // �ִϸ��̼��� ó���� �ǰ� �ִ� ��
            if (normalizedTime >= attackInfoData.ForceTransitionTime)
                TryApplyForce();

            if (normalizedTime >= attackInfoData.ComboTransitionTime)
                TryComboAttack();
        }
        else
        {
            // �ִϸ��̼� �÷��̰� �Ϸ�� ����
            // ���� �޺� �ε����� �����ͼ� �޺������� �����ϴ� ����
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

        target?.TakeDamage(attackInfoData.Damage + stateMachine.Player.Playerconditions.Power);
        target?.TakeEffect(attackInfoData.AttackEffectType, attackInfoData.AttackEffectValue, stateMachine.Player.gameObject);
    }
}
