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
    public GameObject AoEObject;
    private Vector3 position;
    public ParticleSystem ParticleSystem;
    private GameObject breath;
    public float attackTime;
    private ParticleCollision particleCollision;
    public AOEAction() : base()
    {
        ActionType = ActionTypes.AoE;
        //Instantiate(AoEObject);

    }
    public override void OnAwake()
    {
        base.OnAwake();
        position = StateMachine.Enemy.transform.position + StateMachine.Enemy.transform.forward;
        if (breath == null)
        {
            breath = Instantiate(AoEObject, position, Quaternion.identity);
        }
        breath.transform.parent = StateMachine.Enemy.transform;
    }

    public override void OnStart()
    {
        base.OnStart();
        ParticleSystem = breath.GetComponent<ParticleSystem>();
        particleCollision = breath.GetComponent<ParticleCollision>();
        particleCollision.SetDamage(Config.DamageAmount);

        StartAnimation(Config.AnimTriggerHash1); //안움직임 ;;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed)
        {
            StartAnimation(Config.AnimTriggerHash2);
            ParticleSystem.Play();
        }
        attackTime -= Time.deltaTime;
        if (attackTime < 0)
        {
            ParticleSystem.Stop();
            particleCollision.damagable = true;
            isCompleted = true;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }

    public void MakeParticlePoint()
    {
    }
}
