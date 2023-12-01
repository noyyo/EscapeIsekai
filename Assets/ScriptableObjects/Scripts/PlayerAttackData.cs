using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class AttackInfoData
{
    [field: SerializeField] public string AttackName { get; private set; }
    [field: SerializeField] public int ComboStateIndex { get; private set; }
    // �޺��� �����Ƿ��� �������� ������ �ϴ��� = ComboTransitionTime
    // ���� ���� ������ �ϴ��� = ForceTransitionTime
    [field: SerializeField][field: Range(0f, 1f)] public float ComboTransitionTime { get; private set; }    // �޺��� �����Ƿ��� �������� ������ �ϴ���
    [field: SerializeField][field: Range(0f, 3f)] public float ForceTransitionTime { get; private set; }    // �󸶳� ��ư�� �츮�� �־�� �ϴ���..?
    [field: SerializeField][field: Range(-10f, 10f)] public float Force { get; private set; }
    [field: SerializeField] public AttackEffectTypes AttackEffectType { get; private set; }
    [field: SerializeField] public float AttackEffectValue { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
}





[Serializable]
public class PlayerAttackData
{
    // �޺��� ���� ������ ������ ��
    [field: SerializeField] public List<AttackInfoData> AttackInfoDatas { get; private set;}
    // AttackInfo���� ī��Ʈ�� ������ ��
    public int GetAttackInfoCount() { return AttackInfoDatas.Count; }
    // ���� ����ϰ� �ִ� AttackInfoData�� �ε������� ������ ��
    public AttackInfoData GetAttackInfo(int index) { return AttackInfoDatas[index]; }
}
