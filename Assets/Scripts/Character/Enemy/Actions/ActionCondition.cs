using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionCondition
{
    private AttackAction parentAction;
    private EnemyStateMachine stateMachine;
    [Tooltip("�켱������ ������ �׼��� ���డ���� �����϶� �ٸ� �׼ǵ�� ���ؼ� ����� Ȯ���� �����ϴ�. 100�� ��� Ȯ������ ����˴ϴ�.")]
    public static readonly int determinePriority = 100;
    [Range(1, 100)] public int Priority = 1;
    [Header("HpPercentCondition")]
    [Range(0, 100)] public int MoreThanThisHp = 0;
    [Range(0, 100)] public int LessThanThisHp = 100;

    [Header("DistanceCondition")]
    [Range(0f, 100f)] public float MoreThanThisDistance = 0f;
    [Range(0f, 100f)] public float LessThanThisDistance = 100f;

    [Header("ActionCondition")]
    public ActionTypes WhenThisTypeIsActivated = ActionTypes.None;
    public ActionTypes WhenThisTypeIsDeactivated = ActionTypes.None;
    private bool actionCondition;

    [Header("TimeCondition")]
    public int AfterBattleStartTime = 0;
    public int BeforeThisTime = 100000;
    public float CoolDown = 0f;


    public ActionCondition(AttackAction parentAction)
    {
        this.parentAction = parentAction;
        stateMachine = parentAction.StateMachine;
        stateMachine.ActivatedActionsChanged += CheckActionCondition;
        CheckActionCondition();
    }
    // �׼��� ��밡���� �������� Ȯ���մϴ�.
    public bool isEligible()
    {
        Priority = Mathf.Clamp(Priority, 1, 100);
        bool hpCondition = stateMachine.HP >= MoreThanThisHp && stateMachine.HP <= LessThanThisHp;
        bool distanceCondition = stateMachine.TargetDistance >= MoreThanThisDistance && stateMachine.TargetDistance <= LessThanThisDistance;
        bool timeCondition = stateMachine.BattleTime >= AfterBattleStartTime && stateMachine.BattleTime <= BeforeThisTime && Time.time - parentAction.lastUsedTime >= CoolDown;
        return hpCondition && distanceCondition && actionCondition && timeCondition;
    }
    private void CheckActionCondition()
    {
        if (WhenThisTypeIsActivated == ActionTypes.None && WhenThisTypeIsDeactivated == ActionTypes.None)
        {
            actionCondition = true;
            return;
        }

        bool actionActivateCondition = false;
        bool actionDeactivateCondition = true;
        List<AttackAction> activeActions = stateMachine.GetActionsInActive();
        for (int i = 0; i < activeActions.Count; i++)
        {
            if (activeActions[i].ActionType == WhenThisTypeIsActivated)
                actionActivateCondition = true;
            if (activeActions[i].ActionType == WhenThisTypeIsDeactivated)
                actionDeactivateCondition = false;
        }
        if (WhenThisTypeIsActivated == ActionTypes.None)
            actionActivateCondition = true;
        actionCondition = actionActivateCondition && actionDeactivateCondition;
    }

}
