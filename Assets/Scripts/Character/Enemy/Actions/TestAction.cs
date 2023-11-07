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
        Debug.Log($"�����ũ ���� ����ð� : {Time.time}");
    }
    public override void OnStart()
    {
        base.OnStart();
        TestTime = 0f;
        Debug.Log($"��ŸƮ ���� ����ð� : {Time.time}");
    }
    public override void OnEnd()
    {
        base.OnEnd();
        Debug.Log($"���� ���� ����ð� : {Time.time}");
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
        Debug.Log($"����Ʈ ���� �ð�(�׼Ǳ���) : {TestTime}");
    }
    protected override void OnEffectFinish()
    {
        base.OnEffectFinish();
        Debug.Log($"����Ʈ �� �ð�(�׼Ǳ���) : {TestTime}");
    }
}
