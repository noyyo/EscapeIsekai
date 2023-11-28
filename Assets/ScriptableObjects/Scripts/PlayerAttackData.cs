using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class AttackInfoData
{
    [field: SerializeField] public string AttackName { get; private set; }
    [field: SerializeField] public int ComboStateIndex { get; private set; }
    // 콤보가 유지되려면 언제까지 때려야 하는지 = ComboTransitionTime
    // 힘은 언제 적용을 하는지 = ForceTransitionTime
    [field: SerializeField][field: Range(0f, 1f)] public float ComboTransitionTime { get; private set; }
    [field: SerializeField][field: Range(0f, 3f)] public float ForceTransitionTime { get; private set; }
    [field: SerializeField][field: Range(-10f, 10f)] public float Force { get; private set; }
    [field: SerializeField] public AttackEffectTypes AttackEffectType { get; private set; }
    [field: SerializeField] public float AttackEffectValue { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public int Power = 10;

    
}





[Serializable]
public class PlayerAttackData
{
    // 콤보에 대한 정보를 가지고 옴
    [field: SerializeField] public List<AttackInfoData> AttackInfoDatas { get; private set;}
    // AttackInfo에서 카운트를 가지고 옴
    public int GetAttackInfoCount() { return AttackInfoDatas.Count; }
    // 현재 사용하고 있는 AttackInfoData의 인덱스값을 가지고 옴
    public AttackInfoData GetAttackInfo(int index) { return AttackInfoDatas[index]; }
}
