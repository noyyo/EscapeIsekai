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
    public GameObject AoEObject;  //파티클을 가지고 있는 오브젝트
    public float attackTime;  //파티클 재생 시간
    public float StartLifeTime = 2f;  //파티클 생명주기, 클수록 멀리나감
    [Range(300, 1000)] public int MaxParticle = 300;  //startlifertime이 길수록 커야함
    [Range(15f, 65f)] public float Angle = 30f;  //좌우로 설정 각만큼 파티클이 표시됨
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
        shape.angle = Angle;
    }
}
