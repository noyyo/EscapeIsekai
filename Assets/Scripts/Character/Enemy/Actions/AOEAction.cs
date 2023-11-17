using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "AOEActionSO", menuName = "Characters/Enemy/AttackAction/AOEAction")]

public class AOEAction : AttackAction
{
    private Vector3 _position;
    [HideInInspector] public ParticleSystem thisParticleSystem;
    private GameObject _breath;
    private ParticleCollision _particleCollision;
    private bool _nextAnim = true;

    [Header("Particle")]
    [Tooltip("��ƼŬ�� ������ �ִ� ������Ʈ")]
    public GameObject AoEObject;
    [Tooltip("�� ���� ���� �ð�")]
    public float attackTime;  
    [Tooltip("��ƼŬ �����ֱ�, Ŭ���� �ָ������ϴ�. ���� Ŭ���� MaxParticle�� ���ƾ� �մϴ�.")]
    public float StartLifeTime = 2f;  
    [Tooltip("��ƼŬ ���� ũ��")]
    [Range(0.1f, 5f)] public float StartSize = 1f; 
    [Tooltip("�ѹ��� ǥ�õ� �� �ִ� ��ƼŬ ��")]
    [Range(300, 1000)] public int MaxParticle = 300;  
    [Tooltip("������ �������� �¿�� ������ ���Դϴ�.")]
    [Range(15f, 65f)] public float Angle = 30f;  
    public AOEAction() : base()
    {
        ActionType = ActionTypes.AoE;
    }
    public override void OnAwake()
    {
        base.OnAwake();
        Enemy enemy = StateMachine.Enemy;
        enemy.AnimEventReceiver.AnimEventCalled += AnimEventDecision;
        _position = enemy.transform.position + enemy.transform.forward * enemy.Agent.radius + enemy.transform.forward;
        _breath = Instantiate(AoEObject, _position, Quaternion.identity);
        _breath.transform.parent = StateMachine.Enemy.transform;
        thisParticleSystem = _breath.GetComponent<ParticleSystem>();
        SetParticle();
        _particleCollision = _breath.GetComponent<ParticleCollision>();
        _particleCollision.SetDamage(Config.DamageAmount);
    }

    public override void OnStart()
    {
        base.OnStart();
        _particleCollision.damageable = true;
        StartAnimation(Config.AnimTriggerHash1);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed)
        {
            isCompleted = true;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }

    private void AnimEventDecision(AnimationEvent animEvent)
    {
        if (animEvent.stringParameter == "BreathStart")
        {
            thisParticleSystem.Play();
        }
        else if (animEvent.stringParameter == "BreathEnd")
        {
            thisParticleSystem.Stop();
        }
    }
    public void SetParticle()
    {
        ParticleSystem.MainModule main = thisParticleSystem.main;
        ParticleSystem.ShapeModule shape = thisParticleSystem.shape;

        main.startLifetime = StartLifeTime;
        main.maxParticles = MaxParticle;
        main.startSize = StartSize;
        shape.angle = Angle;
    }
}
