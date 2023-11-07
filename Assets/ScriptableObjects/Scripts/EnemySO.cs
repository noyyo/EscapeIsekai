using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Characters/Enemy")]
public class EnemySO : ScriptableObject
{
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
    [field: SerializeField] public float AttackRange { get; private set; } = 1.5f;
    [field: SerializeField] public bool IsFleeable { get; private set; } = false;
    [field: SerializeField] public float FleeThresholdHpRatio { get; private set; } = 0.1f;

    [field: Header("Attack")]
    [field: SerializeField] public int Damage { get; private set; } = 1;
}