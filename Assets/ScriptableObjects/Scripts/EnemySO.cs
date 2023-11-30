using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Characters/Enemy/EnemySO")]
public class EnemySO : ScriptableObject
{
    public int ID;
    public string Name;

    [field: Header("Health")]
    [field: SerializeField] public int MaxHP { get; private set; } = 100;

    [field: Header("Movement")]
    [field: SerializeField] public float WalkSpeed { get; private set; } = 2f;
    [field: SerializeField] public float RunSpeed { get; private set; } = 5f;
    [field: SerializeField] public float RotateSpeed { get; private set; } = 180f;
    [field: SerializeField] public float Acceleration { get; private set; } = 20f;

    [field: Header("AI")]
    [field: SerializeField] public float MinWanderWaitTime { get; private set; } = 3f;
    [field: SerializeField] public float MaxWanderWaitTime { get; private set; } = 6f;
    [field: SerializeField] public float MinWanderDistance { get; private set; } = 2f;
    [field: SerializeField] public float MaxWanderDistance { get; private set; } = 6f;
    [field: SerializeField] public float ChasingRange { get; private set; } = 15f;
    [field: SerializeField] public bool IsFleeable { get; private set; } = false;
    [field: SerializeField] public bool IsMovable { get; private set; } = true;
    [field: SerializeField] public float FleeThresholdHpRatio { get; private set; } = 0.1f;

    [field: Header("Attack")]
    [Tooltip("�� ��Ͽ� �ִ� ����Ʈ�� ������� �� �ְ� �˴ϴ�.")]
    [field: SerializeField] public AttackEffectTypes[] AffectedEffects { get; private set; }
}