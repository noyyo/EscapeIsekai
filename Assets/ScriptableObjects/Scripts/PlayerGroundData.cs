using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class PlayerGroundData
{
    // �⺻���� �̵��ӵ��� ȸ���ӵ� ����
    [field: SerializeField][field: Range(0f, 25f)] public float WalkSpeed { get; private set; } = 1f;
    [field: SerializeField][field: Range(0f, 25f)] public float RunSpeed { get; private set; } = 5f;
    [field: SerializeField][field: Range(0f, 25f)] public float BaseRotatingDamping { get; private set; } = 1f;

    // �⺻ ���¿� �ȱ�, �޸���, �������� ����
    [field: Header("IdleData")]
    [field: Header("WalkData")]
    [field: SerializeField][field: Range(0f, 2f)] public float WalkSpeedModifier { get; private set; } = 0.22f;

    [field: Header("RunData")]
    [field: SerializeField][field: Range(0f, 2f)] public float RunSpeedModifier { get; private set; } = 1f;

    [field: Header("RollData")]
    [field: SerializeField][field: Range(0f, 10f)] public float RollForce { get; private set; } = 10f;

    [field: SerializeField][field: Range(0f, 20f)] public float StaminaCost { get; private set; } = 20f;

    [field: SerializeField][field: Range(0f, 10f)] public float SkillCost { get; private set; } = 10f;
    [field: SerializeField][field: Range(0f, 100f)] public float PowerUpCost { get; private set; } = 100f;
}
