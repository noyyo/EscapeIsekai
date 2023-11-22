using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;

[Serializable]
public class ActionCondition
{
    private AttackAction parentAction;
    private EnemyStateMachine stateMachine;
    [Tooltip("우선순위가 높으면 액션이 실행가능한 상태일때 다른 액션들과 비교해서 실행될 확률이 높습니다. 100인 경우 확정으로 실행됩니다.")]
    public static readonly int determinePriority = 100;
    [Range(1, 100)] public int Priority = 1;
    [Tooltip("해당 값이 True이면 실행가능한 액션 목록에 추가된 뒤 실행될 때까지 목록에서 삭제되지 않습니다.")]
    public bool WaitUntilExecute;
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


    public void Initialize(AttackAction parentAction)
    {
        this.parentAction = parentAction;
        stateMachine = parentAction.StateMachine;
        stateMachine.ActivatedActionsChanged += CheckActionCondition;
        Priority = Mathf.Clamp(Priority, 1, 100);
        CheckActionCondition();
    }
    // 액션이 사용가능한 상태인지 확인합니다. 거리는 확인하지 않습니다.
    public bool isEligible()
    {
        bool hpCondition = stateMachine.HP >= MoreThanThisHp && stateMachine.HP <= LessThanThisHp;
        bool timeCondition = stateMachine.BattleTime >= AfterBattleStartTime && stateMachine.BattleTime <= BeforeThisTime && Time.time - parentAction.lastUsedTime >= CoolDown && !parentAction.HasRemainingEffect;
        if (!parentAction.HasRemainingEffect && parentAction.lastUsedTime == 0f)
            timeCondition = true;
        return hpCondition && actionCondition && timeCondition;
    }
    public bool isSatisfyDistanceCondition()
    {
        return stateMachine.TargetDistance >= MoreThanThisDistance && stateMachine.TargetDistance <= LessThanThisDistance;
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
