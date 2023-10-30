using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComboAttackState : PlayerAttackState
{
    private bool alreadyAppliedForce;
    private bool alreadyApplyCombo;

    AttackInfoData attackInfoData;

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
        if(!stateMachine.IsAttacking) return;
        // ���� ��� ������ �ƴ϶�� true�� ����
        alreadyApplyCombo = true;
    }

    private void TryApplyForce()
    {
        // �̹� ������ ���� ������ ���� �׷��� �ʴٸ� true
        if(alreadyAppliedForce) return;
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
        float normalizedTime = GetNormalizedTime(stateMachine.Player.Animator, "Attack");
        if (normalizedTime < 1f)
        {
            // �ִϸ��̼��� ó���� �ǰ� �ִ� ��
            if(normalizedTime >= attackInfoData.ForceTransitionTime)
                TryApplyForce();

            if(normalizedTime >= attackInfoData.ComboTransitionTime)
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
}
