using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestActionSO", menuName = "Characters/Enemy/AttackAction/TestAction")]
public class TestAction : AttackAction
{
    public float TestTime;
    public override void OnAwake()
    {
        base.OnAwake();
        Debug.Log($"어웨이크 시작 현재시간 : {Time.time}");
    }
    public override void OnStart()
    {
        base.OnStart();
        TestTime = 0f;
        Debug.Log($"스타트 시작 현재시간 : {Time.time}");
    }
    public override void OnEnd()
    {
        base.OnEnd();
        Debug.Log($"엔드 시작 현재시간 : {Time.time}");
    }
    public override void OnUpdate()
    {
        TestTime += Time.deltaTime;
        if (TestTime >= 2f)
        {
            OnEffectStart();
        }
        if (TestTime >= 4f)
        {
            OnEnd();
        }
        base.OnUpdate();
    }
    protected override void OnEffectStart()
    {
        base.OnEffectStart();
        Debug.Log($"이펙트 시작 시간(액션기준) : {TestTime}");
    }
    protected override void OnEffectFinish()
    {
        base.OnEffectFinish();
        Debug.Log($"이펙트 끝 시간(액션기준) : {TestTime}");
    }
}
