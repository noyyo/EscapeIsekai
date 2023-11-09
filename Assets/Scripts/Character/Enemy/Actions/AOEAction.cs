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
    [Tooltip("파티클을 가지고 있는 오브젝트")]
    public GameObject AoEObject;
    [Tooltip("총 패턴 진행 시간")]
    public float attackTime;  
    [Tooltip("파티클 생명주기, 클수록 멀리나갑니다. 값이 클수록 MaxParticle이 많아야 합니다.")]
    public float StartLifeTime = 2f;  
    [Tooltip("파티클 조각 크기")]
    [Range(0.1f, 5f)] public float StartSize = 1f; 
    [Tooltip("한번에 표시될 수 있는 파티클 수")]
    [Range(300, 1000)] public int MaxParticle = 300;  
    [Tooltip("정면을 기준으로 좌우로 퍼지는 각입니다.")]
    [Range(15f, 65f)] public float Angle = 30f;  
    public AOEAction() : base()
    {
        ActionType = ActionTypes.AoE;
    }
    public override void OnAwake()
    {
        base.OnAwake();
        _position = StateMachine.Enemy.transform.position + StateMachine.Enemy.transform.forward;
        if (_breath == null)
        {
            _breath = Instantiate(AoEObject, _position, Quaternion.identity);
        }
        _breath.transform.parent = StateMachine.Enemy.transform;
    }

    public override void OnStart()
    {
        base.OnStart();
        thisParticleSystem = _breath.GetComponent<ParticleSystem>();
        SetParticle();
        _particleCollision = _breath.GetComponent<ParticleCollision>();
        _particleCollision.SetDamage(Config.DamageAmount);
        StartAnimation(Config.AnimTriggerHash1);

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed && _nextAnim)
        {
            _nextAnim = false;
            StartAnimation(Config.AnimTriggerHash2);
            thisParticleSystem.Play();
        }
        attackTime -= Time.deltaTime;
        if (attackTime < 0)
        {
            thisParticleSystem.Stop();
            _particleCollision.damagable = true;
            isCompleted = true;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
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
